using Game_Realtime.Model.Data;

namespace Game_Realtime.Service;

public class ValidatePackageService
{
    private Dictionary<string, int> _monsterPackageCount;
    private List<string> _castlePackageCount;

    public ValidatePackageService()
    {
        _castlePackageCount = new List<string>();
        _monsterPackageCount = new Dictionary<string, int>();
    }

    public bool ValidMonsterPackage(MonsterTakeDamageData data)
    {
        if (_monsterPackageCount.ContainsKey(data.monsterId))
        {
            if (_monsterPackageCount[data.monsterId] < data.indexPackage)
            {
                _monsterPackageCount[data.monsterId] = data.indexPackage;
                return true;
            }
        }
        else
        {
            _monsterPackageCount.Add(data.monsterId,data.indexPackage);
            return true;
        }

        return false;
    }

    public Task KilledMonster(string monsterId)
    {
        _monsterPackageCount.Remove(monsterId);
        return Task.CompletedTask;
    }
    public bool ValidCastlePackage(CastleTakeDamageData data)
    {

        if (_castlePackageCount.Contains(data.monsterId))
        {
            return false;
        }
        else
        {
            _castlePackageCount.Add(data.monsterId);
            return true;
        }

    }
}