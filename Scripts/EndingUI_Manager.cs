using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;


public class EndingUI_Manager : MonoBehaviour
{
    public GameObject textBack; //텍스트 나오는 박스
    public Text storyText;  //나레이션 텍스트
    public GameObject endBtn;   //마치기버튼

    string beforeText;

    private static readonly string[] INVALID_CHARS = {
  " ", "　", "!", "?", "！", "？", "\"", "\'", "\\",
  ".", ",", "、", "。", "…", "・"
};


    void Start()
    {
        beforeText = storyText.text;

        StartCoroutine(TextShowBox());
    }


    IEnumerator TextShowBox()
    {
        yield return new WaitForSeconds(1f);

        textBack.SetActive(true);
        storyText.DOKill();
        storyText.text = "";
        storyText.DOText("뭐지...? 꿈이었나? 꿈이라기엔 너무 생생한데...", 3f).SetEase(Ease.Linear).
            OnUpdate(() => {
                var currentText = storyText.text;
                if (beforeText == currentText)
                    return;

                var newChar = currentText[currentText.Length - 1].ToString();

                if (!INVALID_CHARS.Contains(newChar))
                    SoundMaixerManager.instance.TextSoundPlay();
                beforeText = currentText;
            });

        yield return new WaitForSeconds(3.5f);
        storyText.text = "";
        storyText.DOText("엇! 이건 내가 주웠던 수첩이잖아? 왜 이렇게 낡았지...?\n" +
            "허…! 수첩에 내가 찾았던 유물들이 다 적혀있어!", 5f).SetEase(Ease.Linear).
            OnUpdate(() => {
                var currentText = storyText.text;
                if (beforeText == currentText)
                    return;

                var newChar = currentText[currentText.Length - 1].ToString();

                if (!INVALID_CHARS.Contains(newChar))
                    SoundMaixerManager.instance.TextSoundPlay();
                beforeText = currentText;
            });

        yield return new WaitForSeconds(6f);
        storyText.text = "";
        storyText.DOText("역시 꿈이 아니었어..! 그나저나 이걸 누가 믿어주려나...?", 3f).SetEase(Ease.Linear).
            OnUpdate(() => {
                var currentText = storyText.text;
                if (beforeText == currentText)
                    return;

                var newChar = currentText[currentText.Length - 1].ToString();

                if (!INVALID_CHARS.Contains(newChar))
                    SoundMaixerManager.instance.TextSoundPlay();
                beforeText = currentText;
            });

        yield return new WaitForSeconds(4f);
        textBack.SetActive(false);

        yield return new WaitForSeconds(1f);
        //endBtn.SetActive(true);
    }
}
