using System;
using System.Linq;
using DG.Tweening;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public SaveDataJson saveDataJson;
    // public TextureResources textureResources;
    // public AdsManager adsManager;
    // public GameObject GameUI;
    // public Home Home;
    // public Win win;
    // public Lose lose;
    // public SupportTools supportTools;

    // public LeaderBoardManager leaderBoardManager;
    // public GameObject ListBlockToFind;
    // public GameObject blockToFindPrefab;
    public Movement PlayerBlock;
    // public Setting setting;
    // public GameObject ListTool;
    // public GameObject BackGround3D;
    // public SkeletonGraphic TutorialSkeleton;
    // public SkeletonGraphic CompleteSkeleton;
    // public GameObject TutorialEatBlock;

    // public GameObject SettingBtn;
    // public GameObject StarFrame;
    // public TextMeshProUGUI StarText;

    // public Sale sale;


    private int currentMap = 0;
    // private bool isChallenge = false;
    private BlockCreator blockCreator;

    void Start()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;

        // RectTransform rectBody = Home.transform.parent.parent.GetComponent<RectTransform>();
        // float uiWidthInUnits = rectBody.rect.x * rectBody.transform.localScale.x * -2;
        // float uiHeightInUnits = rectBody.rect.y * rectBody.transform.localScale.y * -2;

        // MeshFilter meshFilter = BackGround3D.GetComponent<MeshFilter>();
        // Vector3 size = meshFilter.mesh.bounds.size;
        // Vector3 scaledSize = Vector3.Scale(size, BackGround3D.transform.localScale);

        // float zDistanceFactor = BackGround3D.transform.position.z / rectBody.transform.position.z;

        // float ScaleX = uiWidthInUnits * zDistanceFactor / scaledSize.x;
        // float ScaleY = uiHeightInUnits * zDistanceFactor / scaledSize.y;
        // float TrueScale = ScaleX >= ScaleY ? ScaleX : ScaleY;

        // BackGround3D.transform.localScale = BackGround3D.transform.localScale * TrueScale;
        Initialize();
    }

    public void SetChallenge(string txt = "")
    {
        // if (txt == "Challenge") isChallenge = true;
        // else isChallenge = false;
    }

    public void Initialize()
    {
        blockCreator = GetComponent<BlockCreator>();
        // adsManager.ShowInterstitialAd(0);

        gameObject.SetActive(true);
        // GameUI.SetActive(true);

        // SetTool();

        // SetListBlockToFind();
        // if (!isChallenge) blockCreator.CreateLever(currentMap);
        // else blockCreator.CreateChallengeLever(currentMap);

        blockCreator.CreateLever(2);
    }

    // void SetTool()
    // {
    //     foreach (Transform child in ListTool.transform)
    //     {
    //         child.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text =
    //             $"{(int)saveDataJson.GetData(child.name)}";
    //     }

    //     StarText.text = "0";
    // }

    // void SetListBlockToFind()
    // {
    //     if (isChallenge) currentMap = (int)saveDataJson.GetData("Challenge");
    //     else currentMap = (int)saveDataJson.GetData("OpenedMap");

    //     // if(!blockCreator.CheckMapLimit(currentMap)) currentMap--;
    //     // int[] RandomRange = saveDataJson.TakeMapData().map[currentMap].RandomRange;

    //     string[] listFind = new string[] { };
    //     int[] valueFind = new int[] { };
    //     if (currentMap < saveDataJson.TakeMapData().map.Length && !isChallenge)
    //     {
    //         listFind = saveDataJson.TakeMapData().map[currentMap].ListFind;
    //         valueFind = saveDataJson.TakeMapData().map[currentMap].ValueFind;
    //     }
    //     else (listFind, valueFind) = SetListItemToFind();

    //     float listFindLength = listFind == null ? 1 : listFind.Length;
    //     float blockSize = 125;
    //     float pos = 0;
    //     float distanceBetweenItem = blockSize / 3;

    //     float ListBlockToFindX = ListBlockToFind.transform.parent.parent.GetComponent<RectTransform>().sizeDelta.x;

    //     if (listFindLength * blockSize < ListBlockToFindX)
    //     {
    //         pos = (ListBlockToFindX - (listFindLength * blockSize)) / (listFindLength + 1);
    //         ListBlockToFind.GetComponent<RectTransform>().sizeDelta = new Vector2(ListBlockToFindX, 0);
    //     }
    //     else
    //     {
    //         ListBlockToFind.GetComponent<RectTransform>().sizeDelta = new Vector2((blockSize + distanceBetweenItem) * listFindLength - distanceBetweenItem, 0);
    //     }

    //     int theme = (int)saveDataJson.GetData("CurrentTheme");

    //     for (int i = 0; i < listFindLength; i++)
    //     {
    //         Transform newBlock = ObjectPoolManager.SpawnObject(blockToFindPrefab, Vector3.zero, Quaternion.identity).transform;
    //         newBlock.gameObject.SetActive(false);
    //         newBlock.SetParent(ListBlockToFind.transform);
    //         newBlock.localScale = Vector3.one;

    //         if (pos == 0) newBlock.localPosition = new Vector3(blockSize / 2 + (blockSize + distanceBetweenItem) * i, 0, 0);
    //         else newBlock.localPosition = new Vector3(blockSize / 2 + pos + (blockSize + pos) * i, 0, 0);

    //         newBlock.name = listFind == null ? "all" : listFind[i];

    //         newBlock.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"0/{valueFind[i]}";

    //         Image img = newBlock.GetChild(0).GetComponent<Image>();
    //         // img.sprite =
    //         //     listFind == null ? textureResources.ListBlockUI.FirstOrDefault(x => x.name == $"all") :
    //         //     textureResources.ListBlockUI.FirstOrDefault(x => x.name == $"{theme}.{newBlock.name}");
    //         img.sprite =
    //             listFind == null ? textureResources.TakeSprite("all") :
    //             textureResources.TakeSprite($"{theme}.{newBlock.name}");
    //         FitItemSizeToParent(img.GetComponent<RectTransform>());
    //     }
    // }

    // public void PlayAnimationItemToFind()
    // {
    //     float listFindLength = ListBlockToFind.transform.childCount;
    //     Transform frame = ListBlockToFind.transform.parent.parent.parent;
    //     Vector3 pos = frame.position;
    //     SettingBtn.transform.localScale = Vector3.zero;
    //     StarFrame.transform.localScale = Vector3.zero;
    //     SettingBtn.SetActive(true);
    //     StarFrame.SetActive(true);
    //     frame.position = new Vector3(0, 0, pos.z);
    //     frame.gameObject.SetActive(true);
    //     int i = 0;
    //     for (; i < listFindLength; i++)
    //     {
    //         Transform item = ListBlockToFind.transform.GetChild(i);
    //         item.gameObject.SetActive(true);
    //         item.localScale = Vector3.zero;

    //         item.DOScale(1, 0.2f).SetEase(Ease.OutBack).SetDelay(0.2f * i + 0.5f);
    //     }

    //     frame.DOMove(pos, 0.5f).SetEase(Ease.InBack).SetDelay(0.2f * i + 0.7f).OnComplete(() =>
    //     {
    //         StarFrame.transform.DOScale(1, 0.2f).SetEase(Ease.OutBack);
    //         SettingBtn.transform.DOScale(1, 0.2f).SetEase(Ease.OutBack)
    //         .OnStart(() =>
    //         {
    //             if (!isChallenge && currentMap == 1)
    //             {
    //                 TutorialEatBlock.SetActive(true);
    //                 TutorialEatBlock.transform.DOScale(1.07f, 0.5f).SetEase(Ease.OutBack).SetLoops(-1, LoopType.Yoyo);
    //                 PlayTutorialAnimation("Left");
    //                 blockCreator.PlayTutorial();
    //             }
    //             else
    //             {
    //                 blockCreator.CreateRandomBlock();
    //                 blockCreator.CheckButterflyBlock();

    //                 if (currentMap == 8)
    //                 {
    //                     // sale.OpenDialog();
    //                     blockCreator.PlayerBlock.GetComponent<Movement>().StopAllAction();
    //                 }
    //             }

    //             // supportTools.CheckToolOpen(isChallenge);
    //             ListTool.SetActive(true);
    //         });
    //         // .OnComplete(() =>
    //         // {
    //         // });
    //     });

    // }

    void FitItemSizeToParent(RectTransform item)
    {
        item.GetComponent<Image>().SetNativeSize();

        Vector2 itemSize = item.sizeDelta;
        // Vector2 imageSizeInCanvas = GetImageSizeInCanvas(item.transform.parent.GetComponent<Image>());
        // Vector2 parentSize = ConvertCanvasSizeToUnityUnits(imageSizeInCanvas);
        Vector2 parentSize = item.transform.parent.GetComponent<RectTransform>().sizeDelta;

        float scaleRatio = 1;
        if (itemSize.x >= itemSize.y && itemSize.x != parentSize.x)
        {
            scaleRatio = parentSize.x / itemSize.x;
        }
        else if (itemSize.y > itemSize.x && itemSize.y != parentSize.y)
        {
            scaleRatio = parentSize.y / itemSize.y;
        }

        item.transform.localScale = new Vector3(Mathf.Abs(scaleRatio), scaleRatio, 1f);
    }

    Vector2 GetImageSizeInCanvas(Image targetImage)
    {
        RectTransform imageRectTransform = targetImage.rectTransform;
        return new Vector2(
            imageRectTransform.rect.width * imageRectTransform.localScale.x,
            imageRectTransform.rect.height * imageRectTransform.localScale.y
        );
    }

    // Vector2 ConvertCanvasSizeToUnityUnits(Vector2 canvasSize)
    // {
    //     RectTransform canvasRectTransform = Home.transform.parent.parent.GetComponent<RectTransform>();
    //     // Lấy tỷ lệ giữa kích thước màn hình và kích thước Canvas
    //     Vector2 screenToCanvasRatio = new Vector2(
    //         Screen.width / canvasRectTransform.rect.width,
    //         Screen.height / canvasRectTransform.rect.height
    //     );

    //     // Chuyển đổi kích thước Canvas sang pixel
    //     Vector2 sizeInPixels = new Vector2(
    //         canvasSize.x * screenToCanvasRatio.x,
    //         canvasSize.y * screenToCanvasRatio.y
    //     );

    //     // Chuyển đổi từ pixel sang Unity units
    //     return sizeInPixels / Screen.dpi * 2.54f; // 2.54 là số cm trong 1 inch
    // }

    public void GameOver()
    {
        ResetListToFind();

        gameObject.SetActive(false);
        // GameUI.SetActive(false);
        // Home.gameObject.SetActive(true);
        // Home.CheckStar();
        // Home.ListBg.SetActive(true);
        // Home.InGameBg.gameObject.SetActive(false);
    }

    void ResetListToFind(int x = 0)
    {
        // if (x == 0)
        // {
        //     SettingBtn.SetActive(false);
        //     StarFrame.SetActive(false);
        //     ListTool.SetActive(false);
        //     ListBlockToFind.transform.parent.parent.parent.gameObject.SetActive(false);
        // }
        // int length = ListBlockToFind.transform.childCount;
        // for (int i = 0; i < 1;)
        // {
        //     length--;
        //     if (length < x) break;
        //     Transform block = ListBlockToFind.transform.GetChild(0);
        //     block.GetChild(1).gameObject.SetActive(true);
        //     block.GetChild(2).gameObject.SetActive(false);
        //     block.name = "BlockUI";
        //     ObjectPoolManager.ReturnObjectToPool(block.gameObject);
        //     block.transform.SetParent(ListBlockToFind.transform.parent);
        // }
    }

    public void CheckScore(string block, int num)
    {
        // Transform item = ListBlockToFind.transform.Find(block);
        // TextMeshProUGUI itemText;
        // if (ListBlockToFind.transform.GetChild(0).name == "all")
        // {
        //     itemText = ListBlockToFind.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
        //     item = ListBlockToFind.transform.GetChild(0);
        // }
        // else if (item == null) return;
        // else itemText = item.GetChild(1).GetComponent<TextMeshProUGUI>();

        // int score = Convert.ToInt32(itemText.text.Split("/")[0]);
        // int total = Convert.ToInt32(itemText.text.Split("/")[1]);
        // score += num;
        // score = score <= total ? score : total;
        // itemText.text = $"{score}/{total}";

        // Transform imageDone = item.GetChild(2);
        // if (score == total && !imageDone.gameObject.activeSelf)
        // {
        //     item.GetChild(1).gameObject.SetActive(false);
        //     Vector3 pos = imageDone.localPosition;
        //     imageDone.gameObject.SetActive(true);
        //     imageDone.localScale = new Vector3(2.5f, 2.5f, 1f);
        //     imageDone.localPosition = Vector3.one;
        //     imageDone.DOScale(1, 0.3f).SetEase(Ease.InCubic).SetDelay(0.2f);
        //     imageDone.DOLocalMove(pos, 0.3f).SetEase(Ease.InCubic).SetDelay(0.2f);
        // }
    }

    public bool CheckWinGame()
    {
        // foreach (Transform child in ListBlockToFind.transform)
        // {
        //     TextMeshProUGUI itemText = child.GetChild(1).GetComponent<TextMeshProUGUI>();
        //     int score = Convert.ToInt32(itemText.text.Split("/")[0]);
        //     int total = Convert.ToInt32(itemText.text.Split("/")[1]);
        //     if (total > score) return false;
        // }

        return false;
    }

    public void PlayAnimationGameCompleted()
    {
        // if (CompleteSkeleton.gameObject.activeSelf) return;
        // // setting.enabledTouch = false;
        // // supportTools.turnOnAllButton = false;
        // StopGame();
        // CompleteSkeleton.gameObject.SetActive(true);
        // CompleteSkeleton.AnimationState.SetAnimation(0, "animation", false);
        // // blockCreator.audioManager.PlaySFX("Complete");

        // Image bg = CompleteSkeleton.transform.parent.GetComponent<Image>();
        // bg.gameObject.SetActive(true);
        // bg.color = new Color(0, 0, 0, 0);
        // bg.DOFade(0.588f, 0.5f);
        // Invoke("PlayBlockAnimationWhenGameCompleted", 2);
    }

    void PlayBlockAnimationWhenGameCompleted()
    {
        // adsManager.ShowInterstitialAd();
        // CompleteSkeleton.gameObject.SetActive(false);
        // Image bg = CompleteSkeleton.transform.parent.GetComponent<Image>();
        // bg.GetComponent<Image>().DOFade(0, 0.5f);
        // bg.gameObject.SetActive(false);

        // blockCreator.PlayCompletedBlockAnimation();
    }

    public void SetGoldPrice()
    {
        // ResetListToFind(1);
        // float ListBlockToFindX = ListBlockToFind.transform.parent.parent.GetComponent<RectTransform>().sizeDelta.x;
        // ListBlockToFind.GetComponent<RectTransform>().sizeDelta = new Vector2(ListBlockToFindX, 0);

        // Transform gold = ListBlockToFind.transform.GetChild(0);
        // gold.name = "Gold";
        // gold.GetChild(1).gameObject.SetActive(true);
        // gold.GetChild(2).gameObject.SetActive(false);
        // gold.localPosition = new Vector3(ListBlockToFindX / 2 , 0, 0);

        // int goldValue = 10;
        // if (isChallenge)
        // {
        //     int mapa = currentMap % 3;
        //     if (mapa == 1) goldValue = 0;
        //     else if (mapa == 0) goldValue = 20;
        // }
        // gold.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"{goldValue}";

        // Image img = gold.GetChild(0).GetComponent<Image>();
        // img.sprite = textureResources.TakeSprite("Gold");
        // FitItemSizeToParent(img.GetComponent<RectTransform>());
    }

    public void AddGoldPrice()
    {
        // Transform gold = ListBlockToFind.transform.GetChild(0);
        // TextMeshProUGUI goldText = gold.GetChild(1).GetComponent<TextMeshProUGUI>();
        // int val = int.Parse(goldText.text) + 1;
        // goldText.text = $"{val}";
        // gold.DOPause();
        // gold.localScale = Vector3.one;
        // gold.DOScale(new Vector3(1,0.9f,1), 0.15f).OnComplete(() =>
        // {
        //     gold.DOScale(1, 0.15f).OnComplete(() =>
        //     {
        //         // WinGame();
        //     });
        // });
    }

    public void AddStarPrice()
    {
    //     Transform star = StarText.transform.parent.GetChild(0);
    //     // TextMeshProUGUI starText = star.GetChild(1).GetComponent<TextMeshProUGUI>();
    //     int val = int.Parse(StarText.text) + 1;
    //     StarText.text = $"{val}";

    //     star.DOPause();
    //     star.localScale = new Vector3(0.6f, 0.6f, 1);
    //     star.DOScale(new Vector3(0.6f,0.45f,1), 0.15f).OnComplete(() =>
    //     {
    //         star.DOScale(0.6f, 0.15f).OnComplete(() =>
    //         {
    //             // WinGame();
    //         });
    //     });
    }

    public void WinGame()
    {
        // if (win.gameObject.activeSelf) return;
        // setting.enabledTouch = true;
        // supportTools.turnOnAllButton = true;
        // StopGame();
        // if (!isChallenge)
        // {
        //     adsManager.LogEvent($"WinMap_{currentMap}");
        //     saveDataJson.SaveData("OpenedMap", ++currentMap);
        //     saveDataJson.SaveData("TakeToolTutorial", false);
        //     saveDataJson.SaveData("Gold", (int)saveDataJson.GetData("Gold") + 10);
        //     saveDataJson.SaveData("Star", (int)saveDataJson.GetData("Star") + blockCreator.totalStar);
        //     // leaderBoardManager.AddScoreAsync((int)saveDataJson.GetData("Star"));
        //     saveDataJson.SaveData("PiggyBank", (int)saveDataJson.GetData("PiggyBank") + 10);
        //     win.PlayWinAnimation();
        // }
        // else
        // {
        //     int mapa = currentMap % 3;


        //     if (mapa == 2) saveDataJson.SaveData("Gold", (int)saveDataJson.GetData("Gold") + 10);
        //     else if (mapa == 0) saveDataJson.SaveData("Gold", (int)saveDataJson.GetData("Gold") + 20);

        //     saveDataJson.SaveData("Butterfly", (int)saveDataJson.GetData("Butterfly") + mapa);

        //     win.PlayWinAnimation(currentMap);
        //     adsManager.LogEvent($"WinMapChallenge_{currentMap}");
        //     saveDataJson.SaveData("Challenge", ++currentMap);
        // }

    }

    public void LoseGame()
    {
        // if (lose.gameObject.activeSelf) return;
        // if(!isChallenge) adsManager.LogEvent($"LoseMap_{currentMap}");
        // StopGame();
        // lose.PlayLoseAnimation();
    }

    bool SupportToolsEnabledTouch;
    public void StopGame()
    {
        // SupportToolsEnabledTouch = supportTools.enabledTouch;
        // supportTools.enabledTouch = false;
        PlayerBlock.enabledTouch = false;
        PlayerBlock.allowMoveDown = false;
    }

    public void ContinueGame()
    {
        // supportTools.enabledTouch = SupportToolsEnabledTouch;
        PlayerBlock.enabledTouch = true;
        PlayerBlock.allowMoveDown = true;
        PlayerBlock.speed = 5;
    }

    public void ReplayGame()
    {
        blockCreator.GameOver();
        ResetListToFind();
        // if(TutorialSkeleton.gameObject.activeSelf) TurnOffTutorial();

        Invoke("Initialize", 0.1f);
    }

    public void GoToHome()
    {
        // blockCreator.GameOver();
        // GameOver();
        // if (TutorialSkeleton.gameObject.activeSelf)
        // {
        //     TutorialSkeleton.gameObject.SetActive(false);
        //     TutorialSkeleton.AnimationState.ClearTracks();
        // }
    }

    public void OpenSettingDialog()
    {
        // if (!setting.enabledTouch) return;
        // setting.PlayShowDialog();
        // StopGame();
    }

    (string[], int[]) SetListItemToFind()
    {
        float mapa = (float)currentMap % 20f;
        switch (mapa)
        {
            case 1: return (new string[] { "1", "4" }, new int[] { 8, 9 });
            case 2: return (new string[] { "5", "6", "7" }, new int[] { 9, 12, 9 });
            case 3: return (new string[] { "5", "3", "4" }, new int[] { 9, 15, 12 });
            case 4: return (new string[] { "7", "4" }, new int[] { 17, 15 });
            case 5: return (new string[] { "3", "4" }, new int[] { 16, 18 });
            case 6: return (new string[] { "6", "7" }, new int[] { 21, 3 });
            case 7: return (new string[] { "3", "4", "5" }, new int[] { 12, 8, 10 });
            case 8: return (null, new int[] { 35 });
            case 9: return (new string[] { "5" }, new int[] { 18 });
            case 10: return (new string[] { "4", "5", "6", "7" }, new int[] { 15, 5, 20, 8 });
            case 11: return (new string[] { "4", "7" }, new int[] { 12, 7 });
            case 12: return (new string[] { "1", "2", "3", "4" }, new int[] { 13, 6, 7, 14 });
            case 13: return (new string[] { "4" }, new int[] { 24 });
            case 14: return (new string[] { "5", "2", "3" }, new int[] { 14, 16, 12 });
            case 15: return (new string[] { "3", "5", "7" }, new int[] { 15, 15, 15 });
            case 16: return (new string[] { "5", "8" }, new int[] { 17, 8 });
            case 17: return (new string[] { "6", "7" }, new int[] { 3, 20 });
            case 18: return (null, new int[] { 50 });
            case 19: return (new string[] { "1", "2" }, new int[] { 10, 10 });
            case 0: return (new string[] { "4", "5", "8", "7" }, new int[] { 21, 15, 18, 7 });
            default: return (null, new int[] { 30 });
        }
    }

    // public void PlayTutorialAnimation(string txt)
    // {
    //     TutorialSkeleton.gameObject.SetActive(true);
    //     TutorialSkeleton.AnimationState.SetAnimation(0, txt, true);
    // }

    // public void TurnOffTutorial()
    // {
    //     TutorialSkeleton.AnimationState.ClearTracks();
    //     TutorialSkeleton.Skeleton.SetToSetupPose();
    //     TutorialSkeleton.gameObject.SetActive(false);
    //     TutorialEatBlock.transform.DOKill();
    //     TutorialEatBlock.SetActive(false);
    // }
}


// ,
//         {
//             "MapName": 10,
//             "BlockList": [
//                 [0, 0, -1, 0, 0, 0, -1, 0, 0],
//                 [0, 0, -1, 0, 0, 0, -1, 0, 0],
//                 [0, 0, -1, -1, 0, -1, -1, 0, 0],
//                 [0, 0, -1, -1, 0, -1, -1, 0, 0],
//                 [0, 0, -1, -1, 0, -1, -1, 0, 0],
//                 [0, 0, -1, -1, 0, -1, -1, 0, 0],
//                 [0, -1, -1, -1, 0, -1, -1, -1, 0],
//                 [0, -1, -1, -1, 0, -1, -1, -1, 0],
//                 [0, -1, -1, -1, 0, -1, -1, -1, 0],
//                 [0, -1, -1, -1, 0, -1, -1, -1, 0]
//             ],
//             "ListFind": null,
//             "ValueFind": [30],
//             "RandomRange": [3,7]
//         }