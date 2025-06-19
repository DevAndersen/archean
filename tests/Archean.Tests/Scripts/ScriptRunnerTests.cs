using Archean.Core.Models.Scripts;
using Archean.Scripts.Services;

namespace Archean.Tests.Scripts;

public class ScriptRunnerTests
{
    [Fact]
    public async Task TickScriptsAsync_RunningScript_TickMethodCalled()
    {
        // Setup
        GlobalScriptRunner runner = new GlobalScriptRunner();
        TestScript script = new TestScript();
        runner.RegisterScript(script);

        // Action
        await runner.TickScriptsAsync();

        // Assertion
        Assert.Equal(1, script.TickCount);
        Assert.False(script.HasStopped);
        Assert.False(script.HasDisposeBeenCalled);
    }

    [Fact]
    public async Task TickScriptsAsync_StoppedScript_TickMethodNotCalled()
    {
        // Setup
        GlobalScriptRunner runner = new GlobalScriptRunner();
        TestScript script = new TestScript();
        runner.RegisterScript(script);
        script.Stop();

        // Action
        await runner.TickScriptsAsync();

        // Assertion
        Assert.Equal(0, script.TickCount);
        Assert.True(script.HasStopped);
        Assert.True(script.HasDisposeBeenCalled);
    }

    [Fact]
    public async Task TickScriptsAsync_ScriptStoppingAfterTick_TickMethodNotCalled()
    {
        // Setup
        GlobalScriptRunner runner = new GlobalScriptRunner();
        TestScript script = new TestScript
        {
            AdditionalTickLogic = script => script.Stop()
        };

        runner.RegisterScript(script);

        // Action
        await runner.TickScriptsAsync();

        // Assertion
        Assert.Equal(1, script.TickCount);
        Assert.True(script.HasStopped);
        Assert.True(script.HasDisposeBeenCalled);
    }

    [Fact]
    public async Task TickScriptsAsync_RunningScript_ScriptNotRemovedFromRunner()
    {
        // Setup
        GlobalScriptRunner runner = new GlobalScriptRunner();
        TestScript script = new TestScript();
        runner.RegisterScript(script);

        // Action
        await runner.TickScriptsAsync();

        // Assertion
        Assert.Equal([script], runner.Scripts);
    }

    [Fact]
    public async Task TickScriptsAsync_StoppedScript_ScriptRemovedFromRunner()
    {
        // Setup
        GlobalScriptRunner runner = new GlobalScriptRunner();
        TestScript script = new TestScript();
        runner.RegisterScript(script);
        script.Stop();

        // Action
        await runner.TickScriptsAsync();

        // Assertion
        Assert.Empty(runner.Scripts);
    }

    private class TestScript : GlobalScript, IDisposable
    {
        public int TickCount { get; private set; }

        public bool HasDisposeBeenCalled { get; private set; }

        public Action<TestScript>? AdditionalTickLogic { get; set; }

        public override Task OnTickAsync()
        {
            TickCount++;
            AdditionalTickLogic?.Invoke(this);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            HasDisposeBeenCalled = true;
        }
    }
}
