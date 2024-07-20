using TMPro;
using UnityEngine;

public class GotchiStatsCard : MonoBehaviour
{
    public TextMeshProUGUI GotchiNameText;
    public TextMeshProUGUI HpText;
    public TextMeshProUGUI AtkText;
    public TextMeshProUGUI CritText;
    public TextMeshProUGUI ApText;
    public TextMeshProUGUI DoubleStrikeText;
    public TextMeshProUGUI CritDamageText;

    private void Update()
    {
        if (GotchiDataManager.Instance.gotchiData.Count <= 0) return;

        var gotchiData = GotchiDataManager.Instance.GetSelectedGotchiData();
        if (gotchiData == null) return;

        // get kinship
        int kinship = gotchiData.kinship;
        int level = gotchiData.level;

        float hp = GotchiStatCalculator.GetPrimaryGameStat(gotchiData.numericTraits[0], TraitType.NRG);
        float attack = GotchiStatCalculator.GetPrimaryGameStat(gotchiData.numericTraits[1], TraitType.AGG);
        float critical = GotchiStatCalculator.GetPrimaryGameStat(gotchiData.numericTraits[2], TraitType.SPK);
        float ap = GotchiStatCalculator.GetPrimaryGameStat(gotchiData.numericTraits[3], TraitType.BRN);
        float doubleStrike = GotchiStatCalculator.GetPrimaryGameStat(gotchiData.numericTraits[4], TraitType.EYS);
        float critDamage = GotchiStatCalculator.GetPrimaryGameStat(gotchiData.numericTraits[5], TraitType.EYC);

        /*
        // iterate over wearables
        foreach (var wearableId in gotchiData.equippedWearables)
        {
            hp += Dropt.Utils.Wearables.GetPrimaryBuff(wearableId, TraitType.NRG);
            attack += Dropt.Utils.Wearables.GetPrimaryBuff(wearableId, TraitType.AGG);
            critical += Dropt.Utils.Wearables.GetPrimaryBuff(wearableId, TraitType.SPK);
            ap += Dropt.Utils.Wearables.GetPrimaryBuff(wearableId, TraitType.BRN);
            // not yet equipment buffs based on primary traits for eyes
            //doubleStrike += Dropt.Utils.Wearables.GetPrimaryBuff(wearableId, TraitType.EYS);
            //critDamage += Dropt.Utils.Wearables.GetPrimaryBuff(wearableId, TraitType.EYC);
        }
        */

        HpText.text = hp.ToString("F0");
        AtkText.text = attack.ToString("F0");
        CritText.text = (critical * 100).ToString("F0") + "%";
        ApText.text = ap.ToString("F0");
        DoubleStrikeText.text = (doubleStrike * 100).ToString("F0") + "%";
        CritDamageText.text = critDamage.ToString("F2") + "x";

        GotchiNameText.text = gotchiData.name + " (" + gotchiData.id + ")";
    }
}