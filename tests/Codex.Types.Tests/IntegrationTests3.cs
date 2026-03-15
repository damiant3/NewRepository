using Codex.Core;
using Xunit;

namespace Codex.Types.Tests
{
    public partial class IntegrationTests  // this file is also locked.  Check IntegrationTestst4.cs or create a new one.
    {

        // --- Proofs (Milestone 10) ---



        [Fact]
        public void Claim_and_refl_proof_succeeds()
        {
            string source =
                "claim zero-is-zero : 0 === 0\n" +
                "proof zero-is-zero = Refl\n";
            DiagnosticBag diag = Helpers.CheckWithProofs(source);
            Assert.DoesNotContain(diag.ToImmutable(), d => d.Severity == DiagnosticSeverity.Error);
        }

        [Fact]
        public void Claim_and_refl_proof_with_types_succeeds()
        {
            string source =
                "claim five-is-five : 5 === 5\n" +
                "proof five-is-five = Refl\n";
            DiagnosticBag diag = Helpers.CheckWithProofs(source);
            Assert.DoesNotContain(diag.ToImmutable(), d => d.Severity == DiagnosticSeverity.Error);
        }

        [Fact]
        public void Refl_proof_with_unequal_sides_fails()
        {
            string source =
                "claim bad : 3 === 5\n" +
                "proof bad = Refl\n";
            DiagnosticBag diag = Helpers.CheckWithProofs(source);
            Assert.Contains(diag.ToImmutable(), d => d.Code == "CDX4010");
        }

        [Fact]
        public void Sym_proof_succeeds()
        {
            string source =
                "claim a-eq : 5 === 5\n" +
                "proof a-eq = Refl\n\n" +
                "claim b-eq : 5 === 5\n" +
                "proof b-eq = sym a-eq\n";
            DiagnosticBag diag = Helpers.CheckWithProofs(source);
            Assert.DoesNotContain(diag.ToImmutable(), d => d.Severity == DiagnosticSeverity.Error);
        }

        [Fact]
        public void Proof_without_claim_fails()
        {
            string source =
                "proof orphan = Refl\n";
            DiagnosticBag diag = Helpers.CheckWithProofs(source);
            Assert.Contains(diag.ToImmutable(), d => d.Code == "CDX4001");
        }

        [Fact]
        public void Type_level_arithmetic_in_claim_normalizes()
        {
            string source =
                "claim add-comm : (3 + 2) === 5\n" +
                "proof add-comm = Refl\n";
            DiagnosticBag diag = Helpers.CheckWithProofs(source);
            Assert.DoesNotContain(diag.ToImmutable(), d => d.Severity == DiagnosticSeverity.Error);
        }

        [Fact]
        public void Type_level_arithmetic_wrong_value_fails()
        {
            string source =
                "claim bad-add : (3 + 2) === 6\n" +
                "proof bad-add = Refl\n";
            DiagnosticBag diag = Helpers.CheckWithProofs(source);
            Assert.Contains(diag.ToImmutable(), d => d.Code == "CDX4010");
        }

        [Fact]
        public void Cong_proof_succeeds()
        {
            string source =
                "claim inner-eq : 5 === 5\n" +
                "proof inner-eq = Refl\n\n" +
                "claim outer-eq : List 5 === List 5\n" +
                "proof outer-eq = cong List inner-eq\n";
            DiagnosticBag diag = Helpers.CheckWithProofs(source);
            Assert.DoesNotContain(diag.ToImmutable(), d => d.Severity == DiagnosticSeverity.Error);
        }

        [Fact]
        public void Induction_base_case_with_refl()
        {
            string source =
                "claim id-nil : Nil === Nil\n" +
                "proof id-nil = Refl\n";
            DiagnosticBag diag = Helpers.CheckWithProofs(source);
            Assert.DoesNotContain(diag.ToImmutable(), d => d.Severity == DiagnosticSeverity.Error);
        }

        [Fact]
        public void Induction_on_list_with_base_case()
        {
            string source =
                "claim list-id (xs) : xs === xs\n" +
                "proof list-id (xs) =\n" +
                "  induction xs\n" +
                "    if Nil -> Refl\n" +
                "    if Cons (head) (tail) -> Refl\n";
            DiagnosticBag diag = Helpers.CheckWithProofs(source);
            Assert.DoesNotContain(diag.ToImmutable(), d => d.Severity == DiagnosticSeverity.Error);
        }

        [Fact]
        public void Trans_proof_chains_equalities()
        {
            string source =
                "claim step-one : 3 === 3\n" +
                "proof step-one = Refl\n\n" +
                "claim step-two : 3 === 3\n" +
                "proof step-two = trans step-one step-one\n";
            DiagnosticBag diag = Helpers.CheckWithProofs(source);
            Assert.DoesNotContain(diag.ToImmutable(), d => d.Severity == DiagnosticSeverity.Error);
        }

        [Fact]
        public void Reverse_reverse_nil_base_case()
        {
            string source =
                "claim rev-nil : reverse (reverse Nil) === Nil\n" +
                "proof rev-nil = assume\n";
            DiagnosticBag diag = Helpers.CheckWithProofs(source);
            Assert.DoesNotContain(diag.ToImmutable(), d => d.Severity == DiagnosticSeverity.Error);
        }

        [Fact]
        public void Reverse_reverse_claim_with_induction()
        {
            string source =
                "claim rev-rev (xs) : reverse (reverse xs) === xs\n\n" +
                "claim rev-nil : reverse (reverse Nil) === Nil\n" +
                "proof rev-nil = assume\n\n" +
                "claim rev-cons (head) (tail) : reverse (reverse (Cons head tail)) === Cons head tail\n" +
                "proof rev-cons (head) (tail) = assume\n\n" +
                "proof rev-rev (xs) =\n" +
                "  induction xs\n" +
                "    if Nil -> rev-nil\n" +
                "    if Cons (head) (tail) -> rev-cons head tail\n";
            DiagnosticBag diag = Helpers.CheckWithProofs(source);
            Assert.DoesNotContain(diag.ToImmutable(), d => d.Severity == DiagnosticSeverity.Error);
        }

        [Fact]
        public void Parameterized_claim_referenced_as_name()
        {
            string source =
                "claim eq-cons (head) (tail) : Cons head tail === Cons head tail\n" +
                "proof eq-cons (head) (tail) = Refl\n\n" +
                "claim use-eq (h) (t) : Cons h t === Cons h t\n" +
                "proof use-eq (h) (t) = eq-cons h t\n";
            DiagnosticBag diag = Helpers.CheckWithProofs(source);
            Assert.DoesNotContain(diag.ToImmutable(), d => d.Severity == DiagnosticSeverity.Error);
        }

        [Fact]
        public void Assume_proof_accepts_any_claim()
        {
            string source =
                "claim anything : 1 === 2\n" +
                "proof anything = assume\n";
            DiagnosticBag diag = Helpers.CheckWithProofs(source);
            Assert.DoesNotContain(diag.ToImmutable(), d => d.Severity == DiagnosticSeverity.Error);
        }

        [Fact]
        public void Induction_missing_cases_fails()
        {
            string source =
                "claim bad-induction (xs) : xs === xs\n" +
                "proof bad-induction (xs) =\n" +
                "  induction xs\n";
            DiagnosticBag diag = Helpers.CheckWithProofs(source);
            Assert.Contains(diag.ToImmutable(), d => d.Code == "CDX4020");
        }

        // --- String/character built-ins ---

        [Fact]
        public void Text_length_type_checks()
        {
            string source =
                "len : Text -> Integer\n" +
                "len (s) = text-length s\n";
            Map<string, CodexType>? types = Helpers.TypeCheck(source);
            Assert.NotNull(types);
            Assert.True(types!.ContainsKey("len"));
        }

        [Fact]
        public void Text_length_compiles_to_csharp()
        {
            string source =
                "main : Integer\n" +
                "main = text-length \"hello\"\n";
            string? cs = Helpers.CompileToCS(source, "strlen");
            Assert.NotNull(cs);
            Assert.Contains(".Length", cs!);
        }

        [Fact]
        public void Char_at_type_checks()
        {
            string source =
                "first-char : Text -> Text\n" +
                "first-char (s) = char-at s 0\n";
            Map<string, CodexType>? types = Helpers.TypeCheck(source);
            Assert.NotNull(types);
            Assert.True(types!.ContainsKey("first-char"));
        }

        [Fact]
        public void Char_at_compiles_to_csharp()
        {
            string source =
                "main : Text\n" +
                "main = char-at \"hello\" 0\n";
            string? cs = Helpers.CompileToCS(source, "charat");
            Assert.NotNull(cs);
            Assert.Contains("ToString()", cs!);
        }

        [Fact]
        public void Substring_type_checks()
        {
            string source =
                "take-three : Text -> Text\n" +
                "take-three (s) = substring s 0 3\n";
            Map<string, CodexType>? types = Helpers.TypeCheck(source);
            Assert.NotNull(types);
            Assert.True(types!.ContainsKey("take-three"));
        }

        [Fact]
        public void Substring_compiles_to_csharp()
        {
            string source =
                "main : Text\n" +
                "main = substring \"hello\" 1 3\n";
            string? cs = Helpers.CompileToCS(source, "substr");
            Assert.NotNull(cs);
            Assert.Contains("Substring", cs!);
        }

        [Fact]
        public void Is_letter_type_checks()
        {
            string source =
                "check : Text -> Boolean\n" +
                "check (s) = is-letter s\n";
            Map<string, CodexType>? types = Helpers.TypeCheck(source);
            Assert.NotNull(types);
            Assert.True(types!.ContainsKey("check"));
        }

        [Fact]
        public void Is_digit_type_checks()
        {
            string source =
                "check : Text -> Boolean\n" +
                "check (s) = is-digit s\n";
            Map<string, CodexType>? types = Helpers.TypeCheck(source);
            Assert.NotNull(types);
            Assert.True(types!.ContainsKey("check"));
        }

        [Fact]
        public void Is_whitespace_type_checks()
        {
            string source =
                "check : Text -> Boolean\n" +
                "check (s) = is-whitespace s\n";
            Map<string, CodexType>? types = Helpers.TypeCheck(source);
            Assert.NotNull(types);
            Assert.True(types!.ContainsKey("check"));
        }

        [Fact]
        public void Text_to_integer_type_checks()
        {
            string source =
                "parse : Text -> Integer\n" +
                "parse (s) = text-to-integer s\n";
            Map<string, CodexType>? types = Helpers.TypeCheck(source);
            Assert.NotNull(types);
            Assert.True(types!.ContainsKey("parse"));
        }

        [Fact]
        public void Char_code_type_checks()
        {
            string source =
                "code : Text -> Integer\n" +
                "code (s) = char-code s\n";
            Map<string, CodexType>? types = Helpers.TypeCheck(source);
            Assert.NotNull(types);
            Assert.True(types!.ContainsKey("code"));
        }

        [Fact]
        public void Code_to_char_type_checks()
        {
            string source =
                "from-code : Integer -> Text\n" +
                "from-code (n) = code-to-char n\n";
            Map<string, CodexType>? types = Helpers.TypeCheck(source);
            Assert.NotNull(types);
            Assert.True(types!.ContainsKey("from-code"));
        }

        [Fact]
        public void String_builtins_compose()
        {
            string source =
                "first-is-digit : Text -> Boolean\n" +
                "first-is-digit (s) = is-digit (char-at s 0)\n\n" +
                "main : Boolean\n" +
                "main = first-is-digit \"3abc\"\n";
            string? cs = Helpers.CompileToCS(source, "compose");
            Assert.NotNull(cs);
        }

        [Fact]
        public void String_ops_sample_compiles()
        {
            string source =
                "count-letters : Text -> Integer -> Integer -> Integer\n" +
                "count-letters (s) (i) (acc) =\n" +
                "  if i >= text-length s\n" +
                "    then acc\n" +
                "    else let ch = char-at s i\n" +
                "      in if is-letter ch\n" +
                "        then count-letters s (i + 1) (acc + 1)\n" +
                "        else count-letters s (i + 1) acc\n\n" +
                "main : Integer\n" +
                "main = count-letters \"hello world\" 0 0\n";
            string? cs = Helpers.CompileToCS(source, "stringops");
            Assert.NotNull(cs);
            Assert.Contains("count_letters", cs!);
            Assert.Contains(".Length", cs);
        }

        [Fact]
        public void Field_access_in_argument_position()
        {
            string source =
                "Pair = record {\n" +
                "  first : Integer,\n" +
                "  second : Integer\n" +
                "}\n\n" +
                "add : Integer -> Integer -> Integer\n" +
                "add (a) (b) = a + b\n\n" +
                "main : Integer\n" +
                "main =\n" +
                "  let p = Pair { first = 3, second = 4 }\n" +
                "  in add p.first p.second\n";
            string? cs = Helpers.CompileToCS(source, "fieldarg");
            Assert.NotNull(cs);
            Assert.Contains("first", cs!);
            Assert.Contains("second", cs);
        }

        [Fact]
        public void Record_field_access_as_function_argument()
        {
            string source =
                "Point = record {\n" +
                "  x : Integer,\n" +
                "  y : Integer\n" +
                "}\n\n" +
                "sum-coords : Point -> Integer\n" +
                "sum-coords (p) = p.x + p.y\n\n" +
                "double : Integer -> Integer\n" +
                "double (n) = n + n\n\n" +
                "main : Integer\n" +
                "main =\n" +
                "  let p = Point { x = 10, y = 20 }\n" +
                "  in double p.x\n";
            string? cs = Helpers.CompileToCS(source, "fieldarg2");
            Assert.NotNull(cs);
        }

        [Fact]
        public void Stage0_lexer_compiles()
        {
            string source =
                "LexState = record {\n" +
                "  source : Text,\n" +
                "  pos : Integer\n" +
                "}\n\n" +
                "at-end : LexState -> Boolean\n" +
                "at-end (s) = s.pos >= text-length s.source\n\n" +
                "peek : LexState -> Text\n" +
                "peek (s) =\n" +
                "  if at-end s then \"\"\n" +
                "  else char-at s.source s.pos\n\n" +
                "main : Boolean\n" +
                "main = at-end (LexState { source = \"hello\", pos = 5 })\n";
            string? cs = Helpers.CompileToCS(source, "lexstate");
            Assert.NotNull(cs);
            Assert.Contains("at_end", cs!);
            Assert.Contains("peek", cs!);
        }
    }
}
