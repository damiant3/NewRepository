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
        Fact def = Fact.CreateDefinition("legacy = 42", "alice", "init");
        m_store.Store(def);
        m_store.UpdateView("my-fn", def.Hash);

        ValueMap<string, ContentHash> canonical = m_store.GetNamedView("canonical");
        ContentHash? hash = null;
        foreach (KeyValuePair<string, ContentHash> kv in canonical)
        {
            if (kv.Key == "my-fn") hash = kv.Value;
        }
        Assert.NotNull(hash);
        Assert.Equal(def.Hash, hash.Value);
    }

    [Fact]
    public void CreateView_empty_name_throws()
    {
        Assert.Throws<ArgumentException>(() => m_store.CreateView(""));
    }

    [Fact]
    public void CreateView_whitespace_name_throws()
    {
        Assert.Throws<ArgumentException>(() => m_store.CreateView("  "));
    }

    [Fact]
    public void CreateView_path_separator_throws()
    {
        Assert.Throws<ArgumentException>(() => m_store.CreateView("../evil"));
    }

    [Fact]
    public void CreateView_dot_name_throws()
    {
        Assert.Throws<ArgumentException>(() => m_store.CreateView(".."));
    }

    [Fact]
    public void GetNamedView_nonexistent_throws()
    {
        Assert.Throws<InvalidOperationException>(() => m_store.GetNamedView("nonexistent"));
    }

    [Fact]
    public void UpdateNamedView_nonexistent_throws()
    {
        Fact def = Fact.CreateDefinition("f x = x", "alice", "init");
        m_store.Store(def);
        Assert.Throws<InvalidOperationException>(
            () => m_store.UpdateNamedView("nonexistent", "f", def.Hash));
    }

    [Fact]
    public void RemoveFromView_nonexistent_view_throws()
    {
        Assert.Throws<InvalidOperationException>(
            () => m_store.RemoveFromView("nonexistent", "f"));
    }

    [Fact]
    public void RemoveFromView_missing_key_is_silent_noop()
    {
        m_store.CreateView("experiment");
        Fact def = Fact.CreateDefinition("f x = x", "alice", "init");
        m_store.Store(def);
        m_store.UpdateNamedView("experiment", "f", def.Hash);

        m_store.RemoveFromView("experiment", "nonexistent-key");

        ValueMap<string, ContentHash> view = m_store.GetNamedView("experiment");
        Assert.Equal(1, view.Count);
    }

    [Fact]
    public void ViewExists_returns_true_for_created_view()
    {
        m_store.CreateView("feature-x");
        Assert.True(m_store.ViewExists("feature-x"));
    }

    [Fact]
    public void ViewExists_returns_false_for_missing_view()
    {
        Assert.False(m_store.ViewExists("nonexistent"));
    }

    [Fact]
    public void ViewExists_returns_true_for_legacy_canonical()
    {
        // FactStore.Init creates view.json, so canonical exists via legacy
        Assert.True(m_store.ViewExists("canonical"));
    }

    [Fact]
    public void CreateView_canonical_with_legacy_throws()
    {
        // FactStore.Init creates view.json — trying to create canonical should fail
        Assert.Throws<InvalidOperationException>(() => m_store.CreateView("canonical"));
    }

    [Fact]
    public void DeleteView_nonexistent_throws()
    {
        Assert.Throws<InvalidOperationException>(() => m_store.DeleteView("nonexistent"));
    }
}

public class ViewConsistencyTests : IDisposable
{
    readonly string m_tempDir;
    readonly FactStore m_store;

    public ViewConsistencyTests()
    {
        m_tempDir = Path.Combine(Path.GetTempPath(), "codex_vc_test_" + Guid.NewGuid().ToString("N")[..8]);
        m_store = FactStore.Init(m_tempDir);
    }

    public void Dispose()
    {
        try { Directory.Delete(m_tempDir, true); } catch { }
    }

    sealed class AlwaysConsistentChecker : IViewConsistencyChecker
    {
        public List<ViewDefinition> ReceivedDefinitions { get; } = [];

        public ViewConsistencyResult Check(IReadOnlyList<ViewDefinition> definitions)
        {
            ReceivedDefinitions.AddRange(definitions);
            return new ViewConsistencyResult(true, []);
        }
    }

    sealed class AlwaysFailsChecker : IViewConsistencyChecker
    {
        public ViewConsistencyResult Check(IReadOnlyList<ViewDefinition> definitions)
        {
            return new ViewConsistencyResult(false, ["type error: cannot unify Integer with Text"]);
        }
    }

    [Fact]
    public void Empty_view_is_consistent()
    {
        m_store.CreateView("empty");
        AlwaysConsistentChecker checker = new();

        ViewConsistencyResult result = m_store.CheckViewConsistency("empty", checker);

        Assert.True(result.IsConsistent);
        Assert.Empty(result.Errors);
        Assert.Empty(checker.ReceivedDefinitions);
    }

    [Fact]
    public void Checker_receives_all_definitions_from_view()
    {
        m_store.CreateView("test-view");
        Fact defA = Fact.CreateDefinition("square x = x * x", "alice", "init");
        Fact defB = Fact.CreateDefinition("double x = x + x", "alice", "init");
        m_store.Store(defA);
        m_store.Store(defB);
        m_store.UpdateNamedView("test-view", "square", defA.Hash);
        m_store.UpdateNamedView("test-view", "double", defB.Hash);

        AlwaysConsistentChecker checker = new();
        ViewConsistencyResult result = m_store.CheckViewConsistency("test-view", checker);

        Assert.True(result.IsConsistent);
        Assert.Equal(2, checker.ReceivedDefinitions.Count);

        List<string> names = checker.ReceivedDefinitions.Select(d => d.Name).OrderBy(n => n).ToList();
        Assert.Equal("double", names[0]);
        Assert.Equal("square", names[1]);

        List<string> sources = checker.ReceivedDefinitions.Select(d => d.Source).OrderBy(s => s).ToList();
        Assert.Contains("double x = x + x", sources);
        Assert.Contains("square x = x * x", sources);
    }

    [Fact]
    public void Inconsistent_view_returns_errors()
    {
        m_store.CreateView("bad-view");
        Fact def = Fact.CreateDefinition("broken = ???", "alice", "init");
        m_store.Store(def);
        m_store.UpdateNamedView("bad-view", "broken", def.Hash);

        AlwaysFailsChecker checker = new();
        ViewConsistencyResult result = m_store.CheckViewConsistency("bad-view", checker);

        Assert.False(result.IsConsistent);
        Assert.Single(result.Errors);
        Assert.Contains("cannot unify", result.Errors[0]);
    }

    [Fact]
    public void Missing_fact_returns_error()
    {
        m_store.CreateView("missing-view");
        ContentHash fakeHash = ContentHash.Of("nonexistent");
        m_store.UpdateNamedView("missing-view", "ghost", fakeHash);

        AlwaysConsistentChecker checker = new();
        ViewConsistencyResult result = m_store.CheckViewConsistency("missing-view", checker);

        Assert.False(result.IsConsistent);
        Assert.Contains(result.Errors, e => e.Contains("missing fact"));
    }

    [Fact]
    public void Non_definition_fact_returns_error()
    {
        m_store.CreateView("wrong-kind");
        ContentHash defHash = ContentHash.Of("some-def");
        Fact trust = Fact.CreateTrust(defHash, TrustDegree.Reviewed, "bob", "ok");
        m_store.Store(trust);
        m_store.UpdateNamedView("wrong-kind", "bad-entry", trust.Hash);

        AlwaysConsistentChecker checker = new();
        ViewConsistencyResult result = m_store.CheckViewConsistency("wrong-kind", checker);

        Assert.False(result.IsConsistent);
        Assert.Contains(result.Errors, e => e.Contains("Trust") && e.Contains("expected Definition"));
    }

    // --- Phase 3: View Composition ---

    Fact StoreDef(string source)
    {
        Fact def = Fact.CreateDefinition(source, "alice", "init");
        m_store.Store(def);
        return def;
    }

    [Fact]
    public void OverrideView_basic()
    {
        Fact defA = StoreDef("a = 1");
        Fact defB = StoreDef("b = 2");
        Fact defA2 = StoreDef("a = 10");

        m_store.CreateView("base");
        m_store.UpdateNamedView("base", "a", defA.Hash);
        m_store.UpdateNamedView("base", "b", defB.Hash);

        ValueMap<string, ContentHash> overrides = ValueMap<string, ContentHash>.s_empty
            .Set("a", defA2.Hash);

        m_store.OverrideView("base", "derived", overrides);

        ValueMap<string, ContentHash> derived = m_store.GetNamedView("derived");
        Assert.Equal(defA2.Hash, derived["a"]);
        Assert.Equal(defB.Hash, derived["b"]);
    }

    [Fact]
    public void OverrideView_adds_new_entry()
    {
        Fact defA = StoreDef("a = 1");
        Fact defC = StoreDef("c = 3");

        m_store.CreateView("base");
        m_store.UpdateNamedView("base", "a", defA.Hash);

        ValueMap<string, ContentHash> overrides = ValueMap<string, ContentHash>.s_empty
            .Set("c", defC.Hash);

        m_store.OverrideView("base", "extended", overrides);

        ValueMap<string, ContentHash> extended = m_store.GetNamedView("extended");
        Assert.Equal(2, extended.Count);
        Assert.Equal(defA.Hash, extended["a"]);
        Assert.Equal(defC.Hash, extended["c"]);
    }

    [Fact]
    public void OverrideView_nonexistent_base_throws()
    {
        ValueMap<string, ContentHash> overrides = ValueMap<string, ContentHash>.s_empty;
        Assert.Throws<InvalidOperationException>(
            () => m_store.OverrideView("nonexistent", "target", overrides));
    }

    [Fact]
    public void OverrideView_target_exists_throws()
    {
        m_store.CreateView("base");
        m_store.CreateView("target");
        ValueMap<string, ContentHash> overrides = ValueMap<string, ContentHash>.s_empty;
        Assert.Throws<InvalidOperationException>(
            () => m_store.OverrideView("base", "target", overrides));
    }

    [Fact]
    public void MergeViews_disjoint()
    {
        Fact defA = StoreDef("a = 1");
        Fact defB = StoreDef("b = 2");

        m_store.CreateView("view-a");
        m_store.UpdateNamedView("view-a", "a", defA.Hash);
        m_store.CreateView("view-b");
        m_store.UpdateNamedView("view-b", "b", defB.Hash);

        ViewMergeResult result = m_store.MergeViews("view-a", "view-b", "merged");

        Assert.True(result.Success);
        Assert.Empty(result.Conflicts);

        ValueMap<string, ContentHash> merged = m_store.GetNamedView("merged");
        Assert.Equal(2, merged.Count);
        Assert.Equal(defA.Hash, merged["a"]);
        Assert.Equal(defB.Hash, merged["b"]);
    }

    [Fact]
    public void MergeViews_overlapping_same_hash()
    {
        Fact defA = StoreDef("a = 1");

        m_store.CreateView("view-a");
        m_store.UpdateNamedView("view-a", "a", defA.Hash);
        m_store.CreateView("view-b");
        m_store.UpdateNamedView("view-b", "a", defA.Hash);

        ViewMergeResult result = m_store.MergeViews("view-a", "view-b", "merged");

        Assert.True(result.Success);
        ValueMap<string, ContentHash> merged = m_store.GetNamedView("merged");
        Assert.Equal(1, merged.Count);
        Assert.Equal(defA.Hash, merged["a"]);
    }

    [Fact]
    public void MergeViews_conflict_different_hash()
    {
        Fact defA1 = StoreDef("a = 1");
        Fact defA2 = StoreDef("a = 2");

        m_store.CreateView("view-a");
        m_store.UpdateNamedView("view-a", "a", defA1.Hash);
        m_store.CreateView("view-b");
        m_store.UpdateNamedView("view-b", "a", defA2.Hash);

        ViewMergeResult result = m_store.MergeViews("view-a", "view-b", "conflict-target");

        Assert.False(result.Success);
        Assert.Single(result.Conflicts);
        Assert.Equal("a", result.Conflicts[0].DefinitionName);
        Assert.Equal(defA1.Hash, result.Conflicts[0].HashA);
        Assert.Equal(defA2.Hash, result.Conflicts[0].HashB);

        // Target should not have been created
        Assert.False(m_store.ViewExists("conflict-target"));
    }

    [Fact]
    public void MergeViews_nonexistent_source_throws()
    {
        m_store.CreateView("view-a");
        Assert.Throws<InvalidOperationException>(
            () => m_store.MergeViews("view-a", "nonexistent", "target"));
    }

    [Fact]
    public void MergeViews_target_exists_throws()
    {
        m_store.CreateView("view-a");
        m_store.CreateView("view-b");
        m_store.CreateView("target");
        Assert.Throws<InvalidOperationException>(
            () => m_store.MergeViews("view-a", "view-b", "target"));
    }

    [Fact]
    public void FilterView_subset()
    {
        Fact defA = StoreDef("a = 1");
        Fact defB = StoreDef("b = 2");
        Fact defC = StoreDef("c = 3");

        m_store.CreateView("full");
        m_store.UpdateNamedView("full", "a", defA.Hash);
        m_store.UpdateNamedView("full", "b", defB.Hash);
        m_store.UpdateNamedView("full", "c", defC.Hash);

        HashSet<string> keep = ["a", "c"];
        m_store.FilterView("full", "filtered", keep);

        ValueMap<string, ContentHash> filtered = m_store.GetNamedView("filtered");
        Assert.Equal(2, filtered.Count);
        Assert.Equal(defA.Hash, filtered["a"]);
        Assert.Equal(defC.Hash, filtered["c"]);
        Assert.Null(filtered["b"]);
    }

    [Fact]
    public void FilterView_empty_filter_creates_empty_view()
    {
        Fact defA = StoreDef("a = 1");
        m_store.CreateView("full");
        m_store.UpdateNamedView("full", "a", defA.Hash);

        HashSet<string> keep = [];
        m_store.FilterView("full", "empty", keep);

        ValueMap<string, ContentHash> filtered = m_store.GetNamedView("empty");
        Assert.Equal(0, filtered.Count);
    }

    [Fact]
    public void FilterView_names_not_in_source_are_ignored()
    {
        Fact defA = StoreDef("a = 1");
        m_store.CreateView("source");
        m_store.UpdateNamedView("source", "a", defA.Hash);

        HashSet<string> keep = ["a", "nonexistent", "also-missing"];
        m_store.FilterView("source", "filtered", keep);

        ValueMap<string, ContentHash> filtered = m_store.GetNamedView("filtered");
        Assert.Equal(1, filtered.Count);
        Assert.Equal(defA.Hash, filtered["a"]);
    }

    [Fact]
    public void FilterView_nonexistent_source_throws()
    {
        HashSet<string> keep = ["a"];
        Assert.Throws<InvalidOperationException>(
            () => m_store.FilterView("nonexistent", "target", keep));
    }

    [Fact]
    public void FilterView_target_exists_throws()
    {
        m_store.CreateView("source");
        m_store.CreateView("target");
        HashSet<string> keep = ["a"];
        Assert.Throws<InvalidOperationException>(
            () => m_store.FilterView("source", "target", keep));
    }
}
