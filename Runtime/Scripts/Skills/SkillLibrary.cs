using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public class SkillLibrary : ObjectLibrary<Skill>
{
    public SkillLibrary() : base()
    {
    }

    public SkillLibrary(string resourceFolder) : base(resourceFolder)
    {
    }

    public Skill GetSkill(string skillID)
    {
        return GetObject(skillID);
    }

    public List<Skill> GetAllSkills()
    {
        return GetAllObjects();
    }

    protected override Skill CreateObjectFromJson(JObject jsonObject)
    {
        return new Skill(jsonObject);
    }
}