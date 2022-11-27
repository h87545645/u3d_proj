using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Honeti;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FragGameCompleted : PanelBase
{
    public CanvasGroup root;

    public TextMeshProUGUI continueText;

    public I18NTextMesh recordText;

    private bool _showReady = false;
    // Start is called before the first frame update
    void Start()
    {
        GetControl<Button>("continueBtn").onClick.AddListener(() =>
        {
            if (_showReady)
            {
                SceneMgr.GetInstance().LoadScene("FragMenuScene",null);
            }
        });
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnFadeIn()
    {
        root.gameObject.SetActive(true);
        root.DOFade(1,0.5f);
        StartCoroutine(UnityUtils.DelayFuc(() =>
        {
            Sequence sq = DOTween.Sequence();
            sq.Append(continueText.DOFade(1, 0.1f));
            sq.Append(continueText.DOFade(0, 0.1f));
            sq.SetLoops(-1);
            _showReady = true;
        },2f));
    }

    private void InitRecord()
    {
        string[] record = new string[]
        {
            UnityUtils.TimeToStringHMS(FragGameRecord.GetInstance().history.playerTotalTime),
            FragGameRecord.GetInstance().history.jumpCnt.ToString(),
        };
        recordText._params = record;
        recordText.updateTranslation();
    }
}
