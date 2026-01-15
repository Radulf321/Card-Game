using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public class EnemyLibrary : ObjectLibrary<Enemy>
{
    public EnemyLibrary() : base()
    {
    }

    public EnemyLibrary(string resourceFolder) : base(resourceFolder)
    {
    }

    public Enemy GetEnemy(string enemyID)
    {
        return GetObject(enemyID);
    }

    public List<Enemy> GetAllEnemies()
    {
        return GetAllObjects();
    }

    protected override Enemy CreateObjectFromJson(JObject jsonObject)
    {
        return new Enemy(jsonObject);
    }
}