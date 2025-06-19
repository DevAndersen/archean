using Archean.Core.Models.Scripts;

namespace Archean.Scripts.Services;

public abstract class ScriptRunner<TScript> where TScript : Script
{
    private readonly List<TScript> _scripts = [];

    public IReadOnlyList<TScript> Scripts => _scripts;

    public virtual void RegisterScript(TScript script)
    {
        _scripts.Add(script);
    }

    public async Task TickScriptsAsync()
    {
        foreach (TScript script in _scripts)
        {
            if (!script.HasStopped)
            {
                await script.OnTickAsync();
            }

            if (script.HasStopped && script is IDisposable disposableScript)
            {
                disposableScript.Dispose();
            }
        }

        _scripts.RemoveAll(x => x.HasStopped);
    }
}
