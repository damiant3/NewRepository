using Codex.Core;
using Codex.Syntax;
using Xunit;

namespace Codex.Syntax.Tests;

public class ProseTemplateTests
{
    static (DocumentNode Doc, DiagnosticBag Diags) ParseProse(string source)
    {
        SourceText src = new("test.codex", source);
        DiagnosticBag bag = new();
        ProseParser parser = new(src, bag);
        return (parser.ParseDocument(), bag);
    }

    [Fact]
    public void Record_template_produces_type_definition()
    {
        string source =
            "Chapter: Banking\n" +
            "\n" +
            "An Account is a record containing:\n" +
            "- owner : Text\n" +
            "- balance : Integer\n";

        (DocumentNode doc, DiagnosticBag diags) = ParseProse(source);
        Assert.False(diags.HasErrors);
        Assert.Single(doc.TypeDefinitions);
        Assert.Equal("Account", doc.TypeDefinitions[0].Name.Text);

        RecordTypeBody body = Assert.IsType<RecordTypeBody>(doc.TypeDefinitions[0].Body);
        Assert.Equal(2, body.Fields.Count);
        Assert.Equal("owner", body.Fields[0].Name.Text);
        Assert.Equal("balance", body.Fields[1].Name.Text);
    }

    [Fact]
    public void Variant_template_produces_type_definition()
    {
        string source =
            "Chapter: Shapes\n" +
            "\n" +
            "Shape is either:\n" +
            "- Circle (radius : Number)\n" +
            "- Rectangle (width : Number, height : Number)\n" +
            "- Point\n";

        (DocumentNode doc, DiagnosticBag diags) = ParseProse(source);
        Assert.False(diags.HasErrors);
        Assert.Single(doc.TypeDefinitions);
        Assert.Equal("Shape", doc.TypeDefinitions[0].Name.Text);

        VariantTypeBody body = Assert.IsType<VariantTypeBody>(doc.TypeDefinitions[0].Body);
        Assert.Equal(3, body.Constructors.Count);
        Assert.Equal("Circle", body.Constructors[0].Name.Text);
        Assert.Single(body.Constructors[0].Fields);
        Assert.Equal("Rectangle", body.Constructors[1].Name.Text);
        Assert.Equal(2, body.Constructors[1].Fields.Count);
        Assert.Equal("Point", body.Constructors[2].Name.Text);
        Assert.Empty(body.Constructors[2].Fields);
    }

    [Fact]
    public void Record_template_with_article_a()
    {
        string source =
            "Chapter: Data\n" +
            "\n" +
            "A Point is a record containing:\n" +
            "- x : Integer\n" +
            "- y : Integer\n";

        (DocumentNode doc, _) = ParseProse(source);
        Assert.Single(doc.TypeDefinitions);
        Assert.Equal("Point", doc.TypeDefinitions[0].Name.Text);
    }

    [Fact]
    public void Template_with_notation_below()
    {
        string source =
            "Chapter: Banking\n" +
            "\n" +
            "An Account is a record containing:\n" +
            "- owner : Text\n" +
            "- balance : Integer\n" +
            "\n" +
            "    deposit : Account -> Integer -> Account\n" +
            "    deposit (account) (amount) = account\n";

        (DocumentNode doc, DiagnosticBag diags) = ParseProse(source);
        Assert.False(diags.HasErrors);
        Assert.Single(doc.TypeDefinitions);
        Assert.Single(doc.Definitions);
        Assert.Equal("deposit", doc.Definitions[0].Name.Text);
    }

    [Fact]
    public void No_template_match_produces_plain_prose()
    {
        string source =
            "Chapter: Intro\n" +
            "\n" +
            "This is just regular prose.\n" +
            "Nothing special here.\n";

        (DocumentNode doc, _) = ParseProse(source);
        Assert.Empty(doc.TypeDefinitions);
        Assert.Empty(doc.Definitions);
    }

    [Fact]
    public void Template_without_bullets_is_ignored()
    {
        string source =
            "Chapter: Data\n" +
            "\n" +
            "Account is a record containing:\n" +
            "\n" +
            "  Some more prose.\n";

        (DocumentNode doc, _) = ParseProse(source);
        Assert.Empty(doc.TypeDefinitions);
    }

    [Fact]
    public void Record_template_desugars_to_ast()
    {
        string source =
            "Chapter: Data\n" +
            "\n" +
            "An Account is a record containing:\n" +
            "- owner : Text\n" +
            "- balance : Integer\n";

        (DocumentNode doc, _) = ParseProse(source);
        DiagnosticBag diagnostics = new();
        Codex.Ast.Desugarer desugarer = new(diagnostics);
        Codex.Ast.Module module = desugarer.Desugar(doc, "data");

        Assert.False(diagnostics.HasErrors,
            string.Join("; ", diagnostics.ToImmutable()));
        Assert.Single(module.TypeDefinitions);
        Codex.Ast.RecordTypeDef rec =
            Assert.IsType<Codex.Ast.RecordTypeDef>(module.TypeDefinitions[0]);
        Assert.Equal("Account", rec.Name.Value);
        Assert.Equal(2, rec.Fields.Count);
    }
}
