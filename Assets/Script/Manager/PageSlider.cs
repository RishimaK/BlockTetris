using System;
using System.Collections;
using DG.Tweening;
using Spine.Unity;
using UnityEngine;
using UnityEngine.EventSystems;

public class PageSlider : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    // private float pageFirstPositionX = 0;
    // public AudioManager audioManager;

    private int directionValue = 1;
    private float lastPositionX = 0;
    private int pageNumber = 2;
    private int curentPage;
    private bool enabledToClick = true;

    private RectTransform Content;
    private float canvaswidth;
    public GameObject Canvas;
    public GameObject ListPageBtn;
    public GameObject Listfr;

    public RectTransform ListBG;

    public GameObject FrChoosen;

    void Start()
    {
        Content = transform.GetChild(0).GetChild(0).GetComponent<RectTransform>();
        canvaswidth = Canvas.GetComponent<RectTransform>().sizeDelta.x;
        curentPage = pageNumber;
        CheckBtn();
        float _height = Content.rect.size.y;

        foreach (Transform child in Content)
        {
            child.GetComponent<RectTransform>().sizeDelta = new Vector2(canvaswidth, _height);
        }

        Content.localPosition = new Vector2(-canvaswidth * pageNumber, 0);
        // ListPageBtn.transform.GetChild(2).GetComponent<Image>().sprite = Footer_fr2;
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        // pageFirstPositionX = Content.localPosition.x;
        Content.DOKill();
    }

    public void OnDrag(PointerEventData eventData)
    {
        float currentPositionX = Content.localPosition.x;
        if(lastPositionX < currentPositionX) directionValue = -1;
        else directionValue = 1;

        lastPositionX = Content.localPosition.x;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        float contentPosX = Math.Abs(Content.localPosition.x);

        for(int i = 0; i < 1;)
        {
            if(contentPosX > canvaswidth) contentPosX -= canvaswidth;
            else break;
        }

        float xx;
        if(contentPosX < canvaswidth * 0.1)  xx = CheckPrePage(-Content.localPosition.x);
        else if(contentPosX > canvaswidth * 0.9) xx = CheckNextPage(-Content.localPosition.x);
        else xx = directionValue == -1 ? 
            CheckPrePage(-Content.localPosition.x) : CheckNextPage(-Content.localPosition.x);

        Content.DOKill();
        Content.DOAnchorPos(new Vector3(xx, 0, 0), 0.2f);
        ChangeListPageBtn(-xx / canvaswidth);
    }

    float CheckNextPage(float contentPosX)
    {
        float xx = 0;
        if(contentPosX > canvaswidth * 3) xx = canvaswidth * 4;
        else if(contentPosX > canvaswidth * 2) xx = canvaswidth * 3;
        else if(contentPosX > canvaswidth) xx = canvaswidth * 2;
        else if(contentPosX > 0) xx = canvaswidth;
        return -xx;
    }

    float CheckPrePage(float contentPosX)
    {
        float xx = 0;
        if(contentPosX < canvaswidth) xx = 0;
        else if(contentPosX < canvaswidth * 2) xx = canvaswidth;
        else if(contentPosX < canvaswidth * 3) xx = canvaswidth * 2;
        else if(contentPosX < canvaswidth * 4) xx = canvaswidth * 3;
        return -xx;
    }

    void ChangeListPageBtn(float page)
    {
        if(!enabledToClick) return;
        if((int)page == curentPage) return;
        float widthFr = canvaswidth / 6;
        float fistPos = -canvaswidth / 2 + widthFr / 2;
        // float val = curentPage > page ?  widthFr : 0;
        RectTransform fr;
        RectTransform btn;
        Transform icon;
        Vector2 size;
        Vector2 pos;

        bool isMove = false;
        for(int i = 0; i < 5; i++)
        {
            fr = Listfr.transform.GetChild(i).GetComponent<RectTransform>();
            btn = ListPageBtn.transform.GetChild(i).GetComponent<RectTransform>();
            icon = btn.GetChild(0);
  
            if(i == page || i == curentPage) isMove = true;

            if(isMove)
            {
                if (i == page)
                {
                    // handle choosen page
                    size = new Vector2(canvaswidth / 3, fr.sizeDelta.y);
                    pos = new Vector2(fistPos + widthFr / 2, fr.localPosition.y);
                    fr.DOSizeDelta(new Vector2(canvaswidth / 3, fr.sizeDelta.y), 0.2f);
                    fistPos += widthFr;

                    icon.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "Open", false);
                    icon.GetComponent<RectTransform>().DOAnchorPos(new Vector3(0, 80, 0), 0.2f);
                    icon.GetComponent<RectTransform>().DOScale(new Vector3(1.1f, 1.1f, 1), 0.2f).SetEase(Ease.OutBounce);
                    btn.GetChild(1).gameObject.SetActive(true);

                    if(icon.GetComponent<SkeletonGraphic>().Skeleton.Data.FindAnimation("Open_idle") != null)
                    {
                        StartCoroutine(PlayIdleAnimation(icon));
                    }
                }
                else
                {
                    size = new Vector2(widthFr, fr.sizeDelta.y);
                    pos = new Vector2(fistPos, fr.localPosition.y);
                    if(i == curentPage) 
                    {
                        fr.DOSizeDelta(size, 0.2f);
                        icon.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "Close", false);
                        icon.GetComponent<RectTransform>().DOAnchorPos(new Vector3(0, 0, 0), 0.2f);
                        icon.GetComponent<RectTransform>().DOScale(new Vector3(0.7f, 0.7f, 1), 0.2f).SetEase(Ease.OutBounce);;
                        btn.GetChild(1).gameObject.SetActive(false);
                    }
                }

                fr.DOAnchorPos(pos, 0.2f);
                btn.sizeDelta = size;
                btn.localPosition = pos;
            }
            fistPos += widthFr;
        }

        FrChoosen.GetComponent<RectTransform>().DOAnchorPos(new Vector3(0, -10, 0) +
            ListPageBtn.transform.GetChild((int)page).GetComponent<RectTransform>().localPosition, 0.2f);

        float timer = 0.2f / Mathf.Abs(page - curentPage);
        ListBG.DOKill();
        float BgWidth = ListBG.GetChild(0).GetComponent<RectTransform>().rect.size.x;
        ListBG.DOAnchorPos(new Vector3(BgWidth * 2 - BgWidth * page, 0, 0), timer);
        // if(page == 4) 
        //     ListBG.DOAnchorPos(new Vector3(-ListBG.GetChild(0).GetComponent<RectTransform>().rect.size.x, 0, 0), timer).SetDelay(0.2f - timer);
        // else if (curentPage == 4) ListBG.DOAnchorPos(new Vector3(0, 0, 0), timer);
        curentPage = (int)page;
    }

    IEnumerator PlayIdleAnimation(Transform anim)
    {
        yield return new WaitForSeconds(0.733f);
        // anim.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "Open_idle", true);
    }

    void CheckBtn()
    {
        float _width = canvaswidth / 6;
        float _height = ListPageBtn.GetComponent<RectTransform>().rect.size.y;

        float fistPos = -canvaswidth / 2 + _width / 2;
        Transform child;
        RectTransform rect;
        RectTransform rectfr;
        for(int i = 0; i < ListPageBtn.transform.childCount; i++)
        {
            child = ListPageBtn.transform.GetChild(i);
            rect = child.GetComponent<RectTransform>();
            rectfr = Listfr.transform.GetChild(i).GetComponent<RectTransform>();

            if(i == pageNumber)
            {
                // child.GetChild(0).GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "Open_idle", true);
                rect.sizeDelta = new Vector2(_width * 2, rect.sizeDelta.y);
                rect.localPosition = new Vector2(fistPos + _width / 2, 0);
                child.GetChild(0).localScale = new Vector3(1.1f, 1.1f, 1);
                fistPos += _width * 2;
            }
            else
            {
                // rect.localScale = new Vector3(0.8f, 0.8f, 1);
                rect.sizeDelta = new Vector2(_width, _height);
                rect.localPosition = new Vector2(fistPos, 0);
                fistPos += _width;
            }
            rectfr.sizeDelta = rect.sizeDelta;
            rectfr.localPosition = rect.localPosition;
        }
        FrChoosen.GetComponent<RectTransform>().sizeDelta = new Vector2(_width * 2, _height);
        FrChoosen.GetComponent<RectTransform>().localPosition = new Vector2(0, -10);
        FrChoosen.SetActive(true);
    }

    public void ChangeMainPage(GameObject btn)
    {
        int index = btn.transform.GetSiblingIndex();
        if(index == curentPage) return;
        // audioManager.PlaySFX("click");

        Content.DOKill();
        Content.DOAnchorPos(new Vector3(-canvaswidth * index, 0, 0), 0.2f);
        ChangeListPageBtn(index);
    }
}
