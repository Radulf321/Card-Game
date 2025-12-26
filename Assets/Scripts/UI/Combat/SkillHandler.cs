using UnityEngine;
using UnityEngine.UI;

#nullable enable

public class SkillHandler : MonoBehaviour
{
    public GameObject chargePrefab;

    private Skill? skill;
    private bool update = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Game.Instance.SubscribeToTriggerMessages(OnTriggerMessage);
    }

    void OnDestroy()
    {
        Game.Instance.UnsubscribeFromTriggerMessages(OnTriggerMessage);
    }

    // Update is called once per frame
    void Update()
    {
        if (update && skill != null)
        {
            transform.Find("Progress").GetComponent<Image>().fillAmount = skill.GetProgressPercentual();
            int charges = skill.GetCharges();
            Image skillImage = transform.Find("SkillImageContainer").Find("SkillImage").GetComponent<Image>();
            skillImage.sprite = Resources.Load<Sprite>(skill.GetImagePath(charges == 0));

            Transform chargesArea = transform.Find("ChargesArea");
            if (charges != chargesArea.childCount) {
                for (int i = 0; i < chargesArea.childCount; i++)
                {
                    Destroy(chargesArea.GetChild(i).gameObject);
                }

                for (int i = 0; i < charges; i++)
                {
                    GameObject chargeInstance = Instantiate(chargePrefab);
                    chargeInstance.transform.SetParent(chargesArea, false);
                    float rotation = ((i + 1) / charges) * 2 * Mathf.PI;
                    chargeInstance.transform.localPosition = new Vector3(Mathf.Sin((float)rotation) * 68.5f, Mathf.Cos((float)rotation) * 68.5f, 0);
                }
            }
            update = false;
        }
    }

    public void SetSkill(Skill skill)
    {
        this.skill = skill;
        update = true;
    }

    public Skill? GetSkill()
    {
        return this.skill;
    }

    private void OnTriggerMessage(TriggerMessage message)
    {
        if (message.GetTriggerType() == TriggerType.SkillProgressChanged)
        {
            Skill? msgSkill = message.GetData().GetSkill();
            if (msgSkill != null && msgSkill.GetID() == this.skill?.GetID())
            {
                update = true;
            }
        }
    }
}
