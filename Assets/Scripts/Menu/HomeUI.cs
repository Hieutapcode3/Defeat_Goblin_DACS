using System;
using System.Collections.Generic;
using AssetKits.ParticleImage;
using TMPro;
using UnityEngine.UI;

public class HomeUI : BaseUI
{
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI goldEarnInSecond;
    public List<TextMeshProUGUI> missionLevel;
    public Button btnBuySlot;
    public Button btnBuyChar;
    public Button btnBuyPet;
    public Button btnFight;
    public Image deleteChar;
    public ParticleImage GoldDecreaseParticle;
    public Image notEnoughGoldsText;
    public Image notHaveCharText;
    public Image goldGroup;
    public override void Init(UIManager uIManager)
    {
        base.Init(uIManager);
        notEnoughGoldsText.gameObject.SetActive(false);
    }
    public override void Show()
    {
        base.Show();
    }

    public override void Hide(Action onComplete = null)
    {
        base.Hide(onComplete);
    }
}