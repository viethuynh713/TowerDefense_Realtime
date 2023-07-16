using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Game_Realtime.Service.WaveService;

public class WaveService
{
    private const string FilePath = "GameConfig/WaveConfig.json";

    private int _currentWaveIndex;

    private Wave _currentWave;
    
    private readonly Dictionary<int, Wave> waves;
    
    public WaveService()
    {
        string jsonString = File.ReadAllText(FilePath);

        waves = JsonConvert.DeserializeObject<Dictionary<int, Wave>>(jsonString) ?? throw new InvalidOperationException();
        
        _currentWaveIndex = 0;

        _currentWave = waves[_currentWaveIndex];
    }

    public Task NextWave()
    {
        Thread.Sleep(10000);
        _currentWaveIndex++;
        if (waves.Count <= _currentWaveIndex)
        {
            _currentWave = waves.Last().Value;
            _currentWave.UpgradeData();
        }
        else
        {
            _currentWave = waves[_currentWaveIndex];
        }

        return Task.CompletedTask;

    }

    public Wave GetCurrentWave()
    {
        return _currentWave;
    }

    public float UpdateWaveTime(float time)
    {
        _currentWave.currentTime -= time;
        return _currentWave.currentTime;

    }

}