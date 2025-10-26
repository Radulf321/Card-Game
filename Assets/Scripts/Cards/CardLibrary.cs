using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public class CardLibrary : ObjectLibrary<Card>
{
    public CardLibrary() : base()
    {
    }

    public CardLibrary(string resourceFolder) : base(resourceFolder)
    {
    }

    public Card GetCard(string cardID)
    {
        return GetObject(cardID);
    }

    public List<Card> GetAllCards()
    {
        return GetAllObjects();
    }

    protected override Card CreateObjectFromJson(JObject jsonObject)
    {
        return new Card(jsonObject);
    }
}