using UnityEngine;

public class SkillAreaHandler : MonoBehaviour
{
    public GameObject skillPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (Skill skill in Game.Instance.GetPlayer().GetSkills()) {
            GameObject skillObject = Instantiate(skillPrefab);
            skillObject.transform.SetParent(this.transform, false);
            skillObject.GetComponent<SkillHandler>().SetSkill(skill);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
