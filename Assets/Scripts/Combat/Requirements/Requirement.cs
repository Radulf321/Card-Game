using System.Threading.Tasks;

abstract public class Requirement
{
    abstract public bool IsFulfilled();
    abstract public Task<string> GetDescription();
}
