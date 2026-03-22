using Codex.Core;
using Codex.Repository;
using Xunit;

namespace Codex.Repository.Tests;

public class ViewTests : IDisposable
{
    readonly string m_tempDir;
    readonly FactStore m_store;

    public ViewTests()
    {
        m_tempDir = Path.Combine(Path.GetTempPath(), "codex_view_test_" + Guid.NewGuid().ToString("N")[..8]);
        m_store = FactStore.Init(m_tempDir);
    }

    public void Dispose()
    {
        try { Directory.Delete(m_tempDir, true); } catch { }
    }

    [Fact]
    public void Default_current_view_is_canonical()
    {
        Assert.Equal("canonical", m_store.GetCurrentViewName());
    }

    [Fact]
    public void CreateView_creates_empty_view()
    {
        m_store.CreateView("feature-x");
        IReadOnlyList<ViewInfo> views = m_store.ListViews();
        Assert.Contains(views, v => v.Name == "feature-x" && v.DefinitionCount == 0);
    }

    [Fact]
    public void CreateView_duplicate_throws()
    {
        m_store.CreateView("feature-x");
        Assert.Throws<InvalidOperationException>(() => m_store.CreateView("feature-x"));
    }

    [Fact]
    public void CreateView_with_copy_copies_current()
    {
        Fact def = Fact.CreateDefinition("square x = x * x", "alice", "init");
        m_store.Store(def);
        m_store.UpdateView("square", def.Hash);

        m_store.CreateView("experiment", copyFromCurrent: true);

        ValueMap<string, ContentHash> experiment = m_store.GetNamedView("experiment");
        ContentHash? hash = null;
        foreach (KeyValuePair<string, ContentHash> kv in experiment)
        {
            if (kv.Key == "square") hash = kv.Value;
        }
        Assert.NotNull(hash);
        Assert.Equal(def.Hash, hash.Value);
    }

    [Fact]
    public void SwitchView_changes_current()
    {
        m_store.CreateView("feature-x");
        m_store.SwitchView("feature-x");
        Assert.Equal("feature-x", m_store.GetCurrentViewName());
    }

    [Fact]
    public void SwitchView_nonexistent_throws()
    {
        Assert.Throws<InvalidOperationException>(() => m_store.SwitchView("nonexistent"));
    }

    [Fact]
    public void DeleteView_removes_view()
    {
        m_store.CreateView("feature-x");
        m_store.DeleteView("feature-x");
        IReadOnlyList<ViewInfo> views = m_store.ListViews();
        Assert.DoesNotContain(views, v => v.Name == "feature-x");
    }

    [Fact]
    public void DeleteView_canonical_throws()
    {
        Assert.Throws<InvalidOperationException>(() => m_store.DeleteView("canonical"));
    }

    [Fact]
    public void DeleteView_resets_current_to_canonical()
    {
        m_store.CreateView("feature-x");
        m_store.SwitchView("feature-x");
        m_store.DeleteView("feature-x");
        Assert.Equal("canonical", m_store.GetCurrentViewName());
    }

    [Fact]
    public void UpdateNamedView_and_GetNamedView_roundtrip()
    {
        m_store.CreateView("experiment");
        Fact def = Fact.CreateDefinition("add x y = x + y", "alice", "init");
        m_store.Store(def);

        m_store.UpdateNamedView("experiment", "add", def.Hash);

        ValueMap<string, ContentHash> view = m_store.GetNamedView("experiment");
        ContentHash? hash = null;
        foreach (KeyValuePair<string, ContentHash> kv in view)
        {
            if (kv.Key == "add") hash = kv.Value;
        }
        Assert.NotNull(hash);
        Assert.Equal(def.Hash, hash.Value);
    }

    [Fact]
    public void RemoveFromView_removes_definition()
    {
        m_store.CreateView("experiment");
        Fact def = Fact.CreateDefinition("f x = x", "alice", "init");
        m_store.Store(def);
        m_store.UpdateNamedView("experiment", "f", def.Hash);

        m_store.RemoveFromView("experiment", "f");

        ValueMap<string, ContentHash> view = m_store.GetNamedView("experiment");
        bool found = false;
        foreach (KeyValuePair<string, ContentHash> kv in view)
        {
            if (kv.Key == "f") found = true;
        }
        Assert.False(found);
    }

    [Fact]
    public void ListViews_marks_current()
    {
        m_store.CreateView("feature-x");
        m_store.SwitchView("feature-x");

        IReadOnlyList<ViewInfo> views = m_store.ListViews();
        ViewInfo? featureView = null;
        foreach (ViewInfo v in views)
        {
            if (v.Name == "feature-x") featureView = v;
        }
        Assert.NotNull(featureView);
        Assert.True(featureView.IsCurrent);
    }

    [Fact]
    public void Named_views_are_independent()
    {
        m_store.CreateView("view-a");
        m_store.CreateView("view-b");

        Fact defA = Fact.CreateDefinition("a = 1", "alice", "init");
        Fact defB = Fact.CreateDefinition("b = 2", "bob", "init");
        m_store.Store(defA);
        m_store.Store(defB);

        m_store.UpdateNamedView("view-a", "x", defA.Hash);
        m_store.UpdateNamedView("view-b", "x", defB.Hash);

        ValueMap<string, ContentHash> viewA = m_store.GetNamedView("view-a");
        ValueMap<string, ContentHash> viewB = m_store.GetNamedView("view-b");

        ContentHash? hashA = null;
        ContentHash? hashB = null;
        foreach (KeyValuePair<string, ContentHash> kv in viewA)
        {
            if (kv.Key == "x") hashA = kv.Value;
        }
        foreach (KeyValuePair<string, ContentHash> kv in viewB)
        {
            if (kv.Key == "x") hashB = kv.Value;
        }

        Assert.NotNull(hashA);
        Assert.NotNull(hashB);
        Assert.NotEqual(hashA.Value, hashB.Value);
    }

    [Fact]
    public void Legacy_view_json_accessible_as_canonical()
    {
        // The existing UpdateView/LookupView writes to view.json (legacy)
        Fact def = Fact.CreateDefinition("legacy = 42", "alice", "init");
        m_store.Store(def);
        m_store.UpdateView("my-fn", def.Hash);

        // Should be accessible via GetNamedView("canonical")
        ValueMap<string, ContentHash> canonical = m_store.GetNamedView("canonical");
        ContentHash? hash = null;
        foreach (KeyValuePair<string, ContentHash> kv in canonical)
        {
            if (kv.Key == "my-fn") hash = kv.Value;
        }
        Assert.NotNull(hash);
        Assert.Equal(def.Hash, hash.Value);
    }
}
