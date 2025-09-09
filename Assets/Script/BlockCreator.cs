// using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using DG.Tweening;
// using MeshSlices;
using Spine.Unity;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class BlockCreator : MonoBehaviour
{
    // [SerializeField] private Material[] blockMaterials; // Các material khác nhau cho từng loại khối
    public SaveDataJson saveDataJson;
    // public AudioManager audioManager;
    public GameObject blockPrefab; // Khối 3D đơn sẽ được sử dụng để tạo các khối Tetris

    [SerializeField] private Material opaqueMaterial;
    [SerializeField] private Material transparentMaterial;
    // [SerializeField] private Material rainbowMaterial;

    public GameObject MapFrame;
    // public GameObject BlockFrame;
    public GameObject ListBlock;
    public Canvas canvas;
    public GameObject PlayerBlock;
    public GameObject GhostBlock;
    public GameObject PoolBlock;
    // public SupportTools supportTools;
    public GameManager gameManager;
    private List<GameObject> ListPlayerBlock = new List<GameObject>();
    // public ObjectPoolManager objectPoolManager;
    int CurrentTheme = 1;
    public TextureResources textureResources;
    // Enum định nghĩa các loại khối Tetris
    public enum BlockType
    {
        //Khối 1
        o,

        //Khối 2
        oo,

        //khối 3
        i,
        l,
        j,

        // khối 4
        I, // Khối I (một hàng 4 ô)
        O, // Khối O (hình vuông 2x2)
        T, // Khối T
        L, // Khối L
        J, // Khối J
        S, // Khối S
        Z,  // Khối Z
    }

    private Vector3 FirstBlockPosition;
    private float blockSize;
    private float blockScale;
    // public int totalStar = 0;
    // public GameObject StarPrefab;
    // public GameObject GoldPrefab;
    // public GameObject ListStar;
    // public GameObject ButterflyList;
    // public GameObject ButterflyPrefab;

    // public GameObject ListEffectEatBlock;
    // public GameObject EffectEatBlockPrefab;
    // public GameObject EffectPool;

    GameObject[,] grid = new GameObject[16, 9];
    Dictionary<GameObject, string> blockTextureMap = new Dictionary<GameObject, string>();
    MaterialPropertyBlock material;
    bool isWinGame = false;

    int[] RandomRange;
    void Start()
    {
        // supportTools = GetComponent<SupportTools>();
        // gameManager = GetComponent<GameManager>();
    }

    void Initialize()
    {
        material = new MaterialPropertyBlock();
        isWinGame = false;
        PlayButterflyEffect = false;
        IsPlayAnimationCompleted = false;
        // GetComponent<SupportTools>().Initialize(FirstBlockPosition, blockSize);
        PlayerBlock.GetComponent<Movement>().Initialize(FirstBlockPosition, blockSize, MapFrame.transform.position.y);
        TurnOffGhostBlock();
    }

    #region Tutorial
    public void PlayTutorial()
    {
        // CreateFirstTutorialBlock();
    }

    void CreateFirstTutorialBlock()
    {
        CreateTetrisBlock(BlockType.o);
        int CurrentTheme = (int)saveDataJson.GetData("CurrentTheme");
        SetBlockImage(PlayerBlock.transform.GetChild(0).gameObject, CurrentTheme, 1);
        PlayerBlock.transform.GetChild(0).name = "1";
        CreateGhostOfPlayerBlock();
        Movement movement = PlayerBlock.GetComponent<Movement>();
        movement.StartMoveDown();
        movement.SetTutorial(1);
    }

    void CreateSecondTutorialBlock()
    {
        // if (!gameManager.TutorialSkeleton.gameObject.activeSelf)
        // {
        //     if (isWinGame) CheckWinGameAnimation();
        //     return;
        // }
        // CreateTetrisBlock(BlockType.l);
        // int CurrentTheme = (int)saveDataJson.GetData("CurrentTheme");
        // SetBlockImage(PlayerBlock.transform.GetChild(0).gameObject, CurrentTheme, 2);
        // PlayerBlock.transform.GetChild(0).name = "2";

        // SetBlockImage(PlayerBlock.transform.GetChild(1).gameObject, CurrentTheme, 4);
        // PlayerBlock.transform.GetChild(1).name = "4";

        // SetBlockImage(PlayerBlock.transform.GetChild(2).gameObject, CurrentTheme, 3);
        // PlayerBlock.transform.GetChild(2).name = "3";

        // CreateGhostOfPlayerBlock();
        // Movement movement = PlayerBlock.GetComponent<Movement>();
        // movement.StartMoveDown();
        // movement.SetTutorial(2);
        // gameManager.PlayTutorialAnimation("Press");
    }

#endregion

#region Spawn Player Block
    public GameObject CreateTetrisBlock(BlockType blockType)
    {
        GameObject tetrisBlock = PlayerBlock;
        tetrisBlock.transform.position = Vector3.zero; // Vị trí bắt đầu của khối Tetris
        tetrisBlock.name = $"{blockType}";
        // Tạo các khối con dựa trên loại khối
        List<Vector3> blockPositions = GetBlockPositions(blockType);
        // int a = 0;
        for(int i = 0; i < blockPositions.Count; i ++)
        {
            int randomTexture = Random.Range(RandomRange[0], RandomRange[1] + 1);
            // a++;
            // if(a == 1 || a == 3) randomTexture = 1;
            // else randomTexture = 3;

            GameObject blockInstance = ObjectPoolManager.SpawnObject(blockPrefab, Vector3.zero, Quaternion.identity);
            blockInstance.GetComponent<TrailRenderer>().enabled = false;

            blockInstance.transform.parent = tetrisBlock.transform;
            blockInstance.transform.localPosition = blockPositions[i];
            blockInstance.transform.localScale = new Vector3(blockScale, blockScale, blockScale);
            SetBlockImage(blockInstance, CurrentTheme, randomTexture);
            blockInstance.name = randomTexture.ToString();
        }

        // Thiết lập tâm của khối tetris
        SetPivotPoint(tetrisBlock, blockType);
        return tetrisBlock;
    }
    
    // Trả về danh sách vị trí các khối con dựa trên loại khối Tetris
    private List<Vector3> GetBlockPositions(BlockType blockType)
    {
        List<Vector3> positions = new List<Vector3>();
        float sizeX = blockSize / 2;
        switch (blockType)
        {
            case BlockType.o:
                positions.Add(new Vector3(0, 0, 0));
                break;

            case BlockType.oo:
                positions.Add(new Vector3(-sizeX, -sizeX, 0));
                positions.Add(new Vector3(sizeX, -sizeX, 0));
                break;

            case BlockType.i:
                positions.Add(new Vector3(-sizeX*2, 0, 0));
                positions.Add(new Vector3(0, 0, 0));
                positions.Add(new Vector3(sizeX*2, 0, 0));
                break;

            case BlockType.l:
                positions.Add(new Vector3(0, sizeX*2, 0));
                positions.Add(new Vector3(0, 0, 0));
                positions.Add(new Vector3(sizeX*2, 0, 0));
                break;

            case BlockType.j:
                positions.Add(new Vector3(0, sizeX*2, 0));
                positions.Add(new Vector3(0, 0, 0));
                positions.Add(new Vector3(-sizeX*2, 0, 0));
                break;

            case BlockType.I:
                // Khối I (một hàng ngang 4 ô)
                positions.Add(new Vector3(-sizeX*3, -sizeX, 0));
                positions.Add(new Vector3(-sizeX, -sizeX, 0));
                positions.Add(new Vector3(sizeX, -sizeX, 0));
                positions.Add(new Vector3(sizeX*3, -sizeX, 0));
                break;
                
            case BlockType.O:
                // Khối O (hình vuông 2x2)
                positions.Add(new Vector3(-sizeX, sizeX, 0));
                positions.Add(new Vector3(sizeX, sizeX, 0));
                positions.Add(new Vector3(-sizeX, -sizeX, 0));
                positions.Add(new Vector3(sizeX, -sizeX, 0));
                break;
                
            case BlockType.T:
                // Khối T
                positions.Add(new Vector3(0, sizeX*2, 0));
                positions.Add(new Vector3(0, 0, 0));
                positions.Add(new Vector3(-sizeX*2, 0, 0));
                positions.Add(new Vector3(sizeX*2, 0, 0));
                break;
                
            case BlockType.L:
                // Khối L
                positions.Add(new Vector3(0, sizeX*2, 0));
                positions.Add(new Vector3(0, 0, 0));
                positions.Add(new Vector3(0, -sizeX*2, 0));
                positions.Add(new Vector3(sizeX*2, -sizeX*2, 0));
                break;
                
            case BlockType.J:
                // Khối J
                positions.Add(new Vector3(0, sizeX*2, 0));
                positions.Add(new Vector3(0, 0, 0));
                positions.Add(new Vector3(0, -sizeX*2, 0));
                positions.Add(new Vector3(-sizeX*2, -sizeX*2, 0));
                break;
                
            case BlockType.S:
                // Khối S
                positions.Add(new Vector3(sizeX*2, sizeX*2, 0));
                positions.Add(new Vector3(0, sizeX*2, 0));
                positions.Add(new Vector3(0, 0, 0));
                positions.Add(new Vector3(-sizeX*2, 0, 0));
                break;
                
            case BlockType.Z:
                // Khối Z
                positions.Add(new Vector3(-sizeX*2, sizeX*2, 0));
                positions.Add(new Vector3(0, sizeX*2, 0));
                positions.Add(new Vector3(0, 0, 0));
                positions.Add(new Vector3(sizeX*2, 0, 0));
                break;
        }
        
        return positions;
    }
    
    // Thiết lập điểm tâm của khối Tetris
    private void SetPivotPoint(GameObject tetrisBlock, BlockType blockType)
    {
        float XX = FirstBlockPosition.x + blockSize * 4;
        // float YY = -FirstBlockPosition.y - GetBlockPositions(blockType)[0].y + MapFrame.transform.position.y*2; triple tetris
        // float YY = FirstBlockPosition.y + GetBlockPositions(blockType)[GetBlockPositions(blockType).Count - 1].y + MapFramePosition.y * 2;
        float YY = FirstBlockPosition.y - GetBlockPositions(blockType)[GetBlockPositions(blockType).Count - 1].y + MapFramePosition.y * 2;
        switch (blockType)
        {
            case BlockType.oo:
            case BlockType.I:
            case BlockType.O:
                tetrisBlock.transform.position = new Vector3(XX + blockSize / 2, YY, FirstBlockPosition.z);
                break;
            default:
                tetrisBlock.transform.position = new Vector3(XX, YY, FirstBlockPosition.z);
                break;
        }
    }

    bool IsPlayAnimationCompleted = false;
    public void CreateRandomBlock()
    {
        // Chọn một loại khối ngẫu nhiên
        // if (!isChallenge && (int)saveDataJson.GetData("OpenedMap") == 1)
        // {
        //     CreateSecondTutorialBlock();
        //     return;
        // }

        if (isWinGame)
        {
            CheckWinGameAnimation();
            return;
        }
        BlockType randomType = (BlockType)Random.Range(0, 12);
        // randomType = BlockType.I;

        // GameObject testBlock = new GameObject("TestBlock");
        // testBlock.transform.SetParent(transform);
        // testBlock.transform.position = Vector3.zero;
        // List<Vector3> blockPositions = GetBlockPositions(randomType);

        // for(int i = 0; i < blockPositions.Count; i ++)
        // {
        //     int randomTexture = Random.Range(RandomRange[0], RandomRange[1] + 1);
        //     GameObject blockInstance = ObjectPoolManager.SpawnObject(blockPrefab, Vector3.zero, Quaternion.identity);
        //     blockInstance.GetComponent<TrailRenderer>().enabled = false;

        //     blockInstance.transform.parent = testBlock.transform;
        //     blockInstance.transform.localPosition = blockPositions[i];
        //     blockInstance.transform.localScale = new Vector3(blockScale, blockScale, blockScale);
        //     SetBlockImage(blockInstance, CurrentTheme, randomTexture);
        //     blockInstance.name = randomTexture.ToString();
        // }

        // SetPivotPoint(testBlock, randomType);

        // testBlock.transform.position = new Vector3(testBlock.transform.position.x - 20, testBlock.transform.position.y, testBlock.transform.position.z);

        // testBlock.transform.DOMove(new Vector3(testBlock.transform.position.x, testBlock.transform.position.y + 100, testBlock.transform.position.z), 20).SetEase(Ease.Linear).OnComplete(() =>
        // {
        //     Destroy(testBlock);
        // });



        CreateTetrisBlock(randomType);

        ListPlayerBlock.Clear();

        CreateGhostOfPlayerBlock();
        PlayerBlock.GetComponent<Movement>().StartMoveDown();
        // supportTools.CheckToolOpen(isChallenge);
    }

    void CheckWinGameAnimation()
    {
        if (!IsPlayAnimationCompleted)
        {
            IsPlayAnimationCompleted = true;
            gameManager.PlayAnimationGameCompleted();
        }
        else
        {
            StartCoroutine(PlayWinGameAnimation());
        }
    }

    public void ChangePlayerBlockToRainbow()
    {
        TurnOffGhostBlock();
        int PlayerBlockLength = PlayerBlock.transform.childCount;
        if (PlayerBlockLength > 1)
        {
            for (int i = 0; i < 1;)
            {
                Transform child = PlayerBlock.transform.GetChild(0);
                DeleteBlock(child.gameObject);
                PlayerBlockLength--;
                if (PlayerBlockLength == 1) break;
            }
        }

        float XX = FirstBlockPosition.x + blockSize * 4;
        float YY = -FirstBlockPosition.y + MapFramePosition.y * 2;

        PlayerBlock.transform.position = new Vector3(XX, YY, PlayerBlock.transform.position.z);
        Transform lastPlayerChild = PlayerBlock.transform.GetChild(0);
        lastPlayerChild.name = "Rainbow";
        lastPlayerChild.localPosition = Vector3.zero;
        SetBlockImage(lastPlayerChild.gameObject, "Rainbow");
    }
#endregion

#region Create Map
    string MapDirection = "";
    void SetRandomRange(int map)
    {
        float mapa = (float)map % 20f;
        MapDirection = "";
        switch (mapa)
        {
            case 1:
                RandomRange = new int[] { 1, 4 };
                break;
            case 2:
                RandomRange = new int[] { 5, 8 };
                MapDirection = "right";
                break;
            case 3:
                RandomRange = new int[] { 2, 5 };
                break;
            case 4:
                RandomRange = new int[] { 4, 7 };
                MapDirection = "left";
                break;
            case 5:
                RandomRange = new int[] { 1, 5 };
                break;
            case 6:
                RandomRange = new int[] { 5, 8 };
                break;
            case 7:
                RandomRange = new int[] { 3, 6 };
                MapDirection = "left";
                break;
            case 8:
                RandomRange = new int[] { 4, 8 };
                break;
            case 9:
                RandomRange = new int[] { 3, 6 };
                MapDirection = "right";
                break;
            case 10:
                RandomRange = new int[] { 3, 7 };
                break;
            case 11:
                RandomRange = new int[] { 4, 7 };
                break;
            case 12:
                RandomRange = new int[] { 1, 4 };
                MapDirection = "left";
                break;
            case 13:
                RandomRange = new int[] { 2, 4 };
                MapDirection = "left";
                break;
            case 14:
                RandomRange = new int[] { 2, 5 };
                break;
            case 15:
                RandomRange = new int[] { 3, 7 };
                break;
            case 16:
                RandomRange = new int[] { 5, 8 };
                MapDirection = "right";
                break;
            case 17:
                RandomRange = new int[] { 5, 8 };
                MapDirection = "right";
                break;
            case 18:
                RandomRange = new int[] { 4, 8 };
                break;
            case 19:
                RandomRange = new int[] { 1, 4 };
                MapDirection = "right";
                break;
            case 0:
                RandomRange = new int[] { 3, 8 };
                break;
        }
    }

    // public bool CheckMapLimit(int currentMap)
    // {
    //     if (currentMap - saveDataJson.TakeMapData().map.Length >= MapList.Length)
    //     {
    //         return false;
    //     }
    //     return true;
    // }

    // bool FirstOpen = false;
    // void SetValue()
    // {
    //     if (FirstOpen) return;
    //     FirstOpen = true;
    // }

    public void CreateLever(int map)
    {
        // isChallenge = false;
        bool isRandomMap = false;
        int[][] blockListData = null;
        // CurrentTheme = (int)saveDataJson.GetData("CurrentTheme");

        if (map < saveDataJson.TakeMapData().map.Length)
        {
            RandomRange = saveDataJson.TakeMapData().map[map].RandomRange;
            blockListData = saveDataJson.TakeMapData().map[map].BlockList;
        }
        else
        {
            isRandomMap = true;
            SetRandomRange(map);
            if (map % 20f == 8 || map % 20f == 16) blockListData = null;
            else
            {
                SaveDataJson.MapShape[] mapList = saveDataJson.TakeMapShapeList().MapList;
                // if (map - saveDataJson.TakeMapData().map.Length < MapList.Length)
                //     blockListData = mapList[MapList[map - saveDataJson.TakeMapData().map.Length]].Map;
                // else
                blockListData = mapList[Random.Range(0, mapList.Length)].Map;
            }
        }

        ListPlayerBlock.Clear();

        // SetValue();
        Vector2 landmarkPocation = GetLandmarkPocation();
        GameObject firstBlock = ObjectPoolManager.SpawnObject(blockPrefab, blockPrefab.transform.position, Quaternion.identity);
        Transform ListBlockTransform = ListBlock.transform;

        if (landmarkPocation.x / landmarkPocation.y > grid.GetLength(1) / grid.GetLength(0))
        {
            blockScale = -landmarkPocation.y * 2 / grid.GetLength(0) / firstBlock.GetComponent<MeshFilter>().mesh.bounds.size.x;
        }
        else blockScale = (-landmarkPocation.x * 2 - 5) / grid.GetLength(1) / firstBlock.GetComponent<MeshFilter>().mesh.bounds.size.x;
        landmarkPocation.x = -blockScale * 8;
        // 5: kich thuoc khung
        // 9: so luong block hang ngang
        firstBlock.transform.localScale = new Vector3(blockScale, blockScale, blockScale);

        blockSize = GetMeshSize(firstBlock.transform).x;
        FirstBlockPosition = new Vector3(landmarkPocation.x, landmarkPocation.y + blockSize / 2 + MapFrame.transform.position.y + 10, 90 + blockScale);
        // BlockFrame.transform.localScale = Vector3.one;
        // float blockFrameScale = (-landmarkPocation.x * 2 + blockSize) / (GetMeshSize(BlockFrame.transform).x - 1.58379f);
        // BlockFrame.transform.localScale = BlockFrame.transform.localScale * blockFrameScale + new Vector3(0, 0, blockFrameScale * 0.5f);
        // BlockFrame.transform.position = new Vector3(0, MapFrame.transform.position.y, 89.8f);
        Initialize();

        if (blockListData == null)
        {
            DeleteBlock(firstBlock);
        }
        else
        {
            int blockListDataHigh = blockListData.Length;
            GameObject newBlock;
            for (int i = grid.GetLength(0) - 1; i >= grid.GetLength(0) - blockListDataHigh; i--)
            {
                for (int j = 0; j < 9; j++)
                {
                    // int randomTexture = Random.Range(1, 8);
                    int x = grid.GetLength(0) - 1 - i;
                    if (blockListData[x][j] == 0) continue;
                    int randomTexture = blockListData[x][j];

                    if (randomTexture == -1 || isRandomMap) randomTexture = Random.Range(RandomRange[0], RandomRange[1] + 1);

                    if (ListBlockTransform.transform.childCount == 0) newBlock = firstBlock;
                    else newBlock = ObjectPoolManager.SpawnObject(blockPrefab, Vector3.zero, Quaternion.identity);

                    newBlock.transform.localScale = firstBlock.transform.localScale;
                    newBlock.transform.position = FirstBlockPosition + new Vector3(j * blockSize, i * blockSize, 0);
                    newBlock.transform.SetParent(ListBlockTransform);
                    SetBlockImage(newBlock, CurrentTheme, randomTexture);
                    // newBlock.GetComponent<Renderer>().material.mainTexture =
                    //     textureResources.ListBlockTexture.FirstOrDefault(x => x.name == $"{CurrentTheme}.{randomTexture}");
                    newBlock.name = randomTexture.ToString();
                    grid[i, j] = newBlock;
                }
            }
            // totalStar = 0;
        }
        // GameObject d = ObjectPoolManager.SpawnObject(blockPrefab, Vector3.zero, Quaternion.identity);
        // d.transform.localScale = firstBlock.transform.localScale;
        // d.transform.position = FirstBlockPosition + new Vector3(0 * blockSize, 0 * blockSize, 0);
        // d.transform.SetParent(ListBlockTransform);


        // CheckButterflyBlock();
        HaveQuestionBlock = false;
        if (map / 20 >= 1)
        {
            switch (map % 20f)
            {
                case 3:
                case 6:
                case 13:
                case 20:
                    HaveQuestionBlock = true;
                    SetQuestionBlock();
                    SetQuestionBlock();
                    SetQuestionBlock();
                    break;
            }
        }

        // int time = MoveBlocksInTheSpecifiedDirection("start");

        // if (map == 1) PlayTutorial();
        // else 
        // Invoke("CreateRandomBlock", 0.1f * time);
        // Invoke("PlayFrameItemToFind", 0.1f * time);
        CreateRandomBlock();
        SetCamera();
    }

    Vector3 MapFramePosition;
    void SetCamera()
    {
        MapFramePosition = MapFrame.transform.position;
        Camera camera = Camera.main;
        camera.transform.position = new Vector3(0, -70, -25);
        camera.transform.rotation = Quaternion.Euler(-35, 0, 0);
    }

    void PlayFrameItemToFind()
    {
        // gameManager.PlayAnimationItemToFind();
    }

    void SetBlockImage(GameObject block, int CurrentTheme, int randomTexture)
    {
        Renderer renderer = block.GetComponent<Renderer>();
        material.Clear();

        // material.SetTexture("_BaseMap", textureResources.ListBlockTexture.FirstOrDefault(x => x.name == $"tt{CurrentTheme}.{randomTexture}"));
        // material.SetTexture("_BumpMap", textureResources.ListBlockTexture.FirstOrDefault(x => x.name == $"NormalMap{CurrentTheme}.{randomTexture}"));
        textureResources.SetTexture(material, $"{CurrentTheme}.{randomTexture}");
        renderer.SetPropertyBlock(material);
    }

    void SetBlockImage(GameObject block, string name)
    {
        Renderer renderer = block.GetComponent<Renderer>();
        // MaterialPropertyBlock material = new MaterialPropertyBlock();
        material.Clear();

        // if (name == "Rainbow")
        // {
        //     renderer.sharedMaterial = rainbowMaterial;
        // }
        // else
        // {
            textureResources.SetTexture(material, $"_{name}");
        // }
        blockTextureMap[block] = $"{name}";
        renderer.SetPropertyBlock(material);
    }

    void SetQuestionBlock()
    {
        int i = Random.Range(0, 11);
        int j = Random.Range(0, 9);
        if (grid[i,j] == null) { SetQuestionBlock(); return; }

        if (blockTextureMap.TryGetValue(grid[i,j], out string textureName) && blockTextureMap[grid[i,j]] == "Question")
        {
            SetQuestionBlock();
            return;
        }

        List<GameObject> nearbyBlocks = new List<GameObject>();
        CheckSameNearbyBlock(nearbyBlocks, grid[i,j], i, j);

        if (nearbyBlocks.Count < 2) { SetQuestionBlock(); return; }

        foreach (GameObject block in nearbyBlocks)
        {
            SetBlockImage(block, "Question");
        }
    }

    bool HaveQuestionBlock = false;
    bool HaveButterflyBlock = false;

    void CheckSpecialBlock(int x, int y)
    {
        if (!HaveQuestionBlock && !HaveButterflyBlock) return;
        
        // Định nghĩa các hướng kiểm tra (right, left, up, down)
        var directions = new (int dx, int dy)[] { (1, 0), (-1, 0), (0, 1), (0, -1) };
        
        foreach (var (dx, dy) in directions)
        {
            int newX = x + dx;
            int newY = y + dy;
            
            // Kiểm tra bounds và null
            if (newX < 0 || newX >= grid.GetLength(0) || 
                newY < 0 || newY >= grid.GetLength(1) || 
                grid[newX, newY] == null)
                continue;
                
            GameObject block = grid[newX, newY];
            
            // Early exit nếu block name là "Butterfly"
            if (block.name == "Butterfly")
            {
                if (HaveButterflyBlock)
                    DeleteButterflyBlock(block, newX, newY);
                continue;
            }
            
            // Chỉ kiểm tra texture nếu có question block
            if (HaveQuestionBlock)
            {
                if (blockTextureMap.TryGetValue(block, out string textureName) && blockTextureMap[block] == "Question")
                {
                    SetBlockImage(block, CurrentTheme, int.Parse(block.name));
                }
            }
        }
    }

    public void CheckButterflyBlock()
    {
        HaveButterflyBlock = false;
        int butterflyCount = (int)saveDataJson.GetData("Butterfly");
        if (butterflyCount <= 0) return;
        HaveButterflyBlock = true;

        int blockCount = 0;
        foreach (GameObject child in grid)
        {
            if (child != null) blockCount++;
        }

        butterflyCount = butterflyCount > blockCount ? blockCount : butterflyCount;
        for (int i = 0; i < butterflyCount;)
        {
            int x = Random.Range(0, grid.GetLength(0));
            int y = Random.Range(0, grid.GetLength(1));
            GameObject child = grid[x, y];
            if (child == null || child.name == "Butterfly") continue;

            SetBlockImage(child, "Butterfly");
            // child.GetComponent<Renderer>().material.mainTexture =
            //     textureResources.ListBlockTexture.FirstOrDefault(x => x.name == "Butterfly");
            StartCoroutine(SpawnParticle(child.transform.position - new Vector3 (0,0,blockSize)));
            child.name = "Butterfly";
            i++;
        }
    }

    bool PlayButterflyEffect = false;
    void DeleteButterflyBlock(GameObject block, int x, int y)
    {
        List<GameObject> nearlyList = new List<GameObject>();
        CheckSameNearbyBlock(nearlyList, block, x, y);
        int nearlyListLength = nearlyList.Count;

        for (int i = 0; i < nearlyListLength; i++)
        {
            GameObject butterflyBlock = nearlyList[i];
            int a = Mathf.RoundToInt((butterflyBlock.transform.position.y - FirstBlockPosition.y) / blockSize);
            int b = Mathf.RoundToInt((butterflyBlock.transform.position.x - FirstBlockPosition.x) / blockSize);
            if (grid[a, b] = null) return;
            saveDataJson.SaveData("Butterfly", (int)saveDataJson.GetData("Butterfly") - 1);
            butterflyBlock.transform.DOScale(butterflyBlock.transform.localScale * 1.2f, 0.1f).SetEase(Ease.OutBounce).OnComplete(() =>
            {
                butterflyBlock.transform.DOScale(Vector3.zero, 0.2f).OnComplete(() =>
                {
                    DeleteBlock(butterflyBlock);
                    PlayButterAnimation(a, b);
                });
            });
            grid[a, b] = null;
        }
        
        PlayButterflyEffect = true;

        // block.transform.DOScale(block.transform.localScale * 1.2f, 0.1f).SetEase(Ease.OutBounce).OnComplete(() => {
        //     block.transform.DOScale(Vector3.zero, 0.2f).OnComplete(() =>
        //     {
        //         DeleteBlock(block);
        //         PlayButterAnimation(x, y);
        //     });
        // });
        // grid[x, y] = null;
    }

    void PlayButterAnimation(int x, int y)
    {
        // GameObject Butterfly = ObjectPoolManager.SpawnObject(ButterflyPrefab, Vector3.zero, Quaternion.identity);
        // Transform butterfly = Butterfly.transform;
        // butterfly.SetParent(ButterflyList.transform);
        // Butterfly.name = "Butterfly";


        // butterfly.position = FirstBlockPosition + new Vector3(y * blockSize, x * blockSize);
        // butterfly.position = new Vector3(butterfly.position.x, butterfly.position.y, 90);
        // butterfly.gameObject.SetActive(true);
        // butterfly.localScale = Vector3.zero;
        // butterfly.DOScale(Vector3.one, 0.1f);
        // StartCoroutine(PlayButterfly(butterfly));
    }

    IEnumerator PlayButterfly(Transform Butterfly)
    {
        yield return new WaitForSeconds(0.1f);
        // audioManager.PlaySFX("Butterfly");
        // Butterfly.GetComponent<ButterflyControl>().PlayButterflyAnimation();
    }

    void SetRockBlock()
    {
        for (int i = 0; i < 3;)
        {
            int x = Random.Range(3, 10);
            int y = Random.Range(0, 9);
            if (grid[x, y] == null)
            {
                GameObject newBlock = ObjectPoolManager.SpawnObject(blockPrefab, Vector3.zero, Quaternion.identity);
                newBlock.transform.position = FirstBlockPosition + new Vector3(y * blockSize, x * blockSize, 0);
                newBlock.transform.localScale = new Vector3(blockScale, blockScale, blockScale);
                newBlock.transform.SetParent(ListBlock.transform);
                grid[x, y] = newBlock;
            }
            else if (grid[x, y].name == "Rock") continue;
            SetBlockImage(grid[x, y], "Rock");
            // grid[x, y].GetComponent<Renderer>().material.mainTexture =
            //     textureResources.ListBlockTexture.FirstOrDefault(x => x.name == "Rock");
            grid[x, y].name = "Rock";
            i++;
        }
    }

    void PushAllBlockUp()
    {
        for (int i = grid.GetLength(0) - 1; i >= 0; i--)
        {
            for (int j = grid.GetLength(1) - 1; j >= 0; j--)
            {
                if (grid[i, j] != null)
                {
                    grid[i, j].transform.position = new Vector3(
                        grid[i, j].transform.position.x,
                        FirstBlockPosition.y + blockSize * (i + 1),
                        grid[i, j].transform.position.z
                    );
                    grid[i + 1, j] = grid[i, j];
                    grid[i, j] = null;
                }

                if (i == 0)
                {
                    GameObject newBlock = ObjectPoolManager.SpawnObject(blockPrefab, Vector3.zero, Quaternion.identity);
                    newBlock.transform.position = FirstBlockPosition + new Vector3(j * blockSize, i * blockSize, 0);
                    newBlock.transform.localScale = new Vector3(blockScale, blockScale, blockScale);
                    newBlock.transform.SetParent(ListBlock.transform);
                    SetBlockImage(newBlock, "Rock");
                    // newBlock.GetComponent<Renderer>().material.mainTexture =
                    //     textureResources.ListBlockTexture.FirstOrDefault(x => x.name == "Rock");
                    newBlock.name = "Rock";
                    grid[i, j] = newBlock;
                }
            }
        }
    }

    #endregion

    #region Functions Caculate
    void DeleteBlock(GameObject block)
    {
        block.name = "Block";
        ObjectPoolManager.ReturnObjectToPool(block);
        block.transform.SetParent(PoolBlock.transform);
    }

    public Vector2 GetCanvasSizeInUnits()
    {
        if (canvas == null)
        {
            Debug.LogError("Canvas not assigned!");
            return Vector2.zero;
        }

        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        Vector2 size = Vector2.zero;

        switch (canvas.renderMode)
        {
            case RenderMode.ScreenSpaceOverlay:
            case RenderMode.ScreenSpaceCamera:
                // Trong chế độ Screen Space, đơn vị là pixels
                size = canvasRect.rect.size;
                Debug.Log("Canvas size in pixels: " + size);

                // Chuyển đổi từ pixels sang đơn vị thế giới nếu có camera
                if (canvas.renderMode == RenderMode.ScreenSpaceCamera && canvas.worldCamera != null)
                {
                    // Lấy khoảng cách từ canvas đến camera
                    float distance = Mathf.Abs(canvas.planeDistance);

                    // Tính kích thước trong đơn vị thế giới
                    Camera cam = canvas.worldCamera;
                    float height = 2.0f * distance * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
                    float width = height * cam.aspect;

                    // Tỷ lệ giữa kích thước pixel và kích thước thế giới
                    float pixelToUnitRatio = height / Screen.height;

                    size.x *= pixelToUnitRatio;
                    size.y *= pixelToUnitRatio;

                    Debug.Log("Canvas size in world units: " + size);
                }
                break;

            case RenderMode.WorldSpace:
                // Trong World Space, đơn vị đã là đơn vị thế giới (unit)
                size = new Vector2(
                    canvasRect.rect.width * canvasRect.localScale.x,
                    canvasRect.rect.height * canvasRect.localScale.y
                );
                Debug.Log("Canvas size in world units: " + size);
                break;
        }

        return size;
    }

    Vector3 GetMeshSize(Transform block)
    {
        MeshFilter meshFilter = block.GetComponent<MeshFilter>();
        if (meshFilter != null && meshFilter.mesh != null)
        {
            // Lấy kích thước nguyên bản của mesh
            Vector3 size = meshFilter.mesh.bounds.size;
            // Debug.Log("Kích thước mesh: " + size);

            // Kích thước thực tế với scale
            Vector3 scaledSize = Vector3.Scale(size, block.transform.localScale);
            // Debug.Log("Kích thước thực tế: " + scaledSize);
            return scaledSize;
        }
        return Vector3.zero;
    }

    Vector2 GetLandmarkPocation()
    {
        RectTransform rectBody = MapFrame.GetComponent<RectTransform>();

        float uiWidthInUnits = rectBody.rect.x * canvas.transform.localScale.x;
        float uiHeightInUnits = rectBody.rect.y * canvas.transform.localScale.y;
        // float uiHeightInUnits = rectBody.rect.y * canvas.transform.localScale.y + rectBody.position.y;

        return new Vector2(uiWidthInUnits, uiHeightInUnits);
    }

    public bool IsValidPosition(Transform block)
    {
        foreach (Transform child in block)
        {
            Vector3 childPosition = child.position;
            int x = Mathf.CeilToInt((childPosition.y - FirstBlockPosition.y) / blockSize);
            int y = Mathf.RoundToInt((childPosition.x - FirstBlockPosition.x) / blockSize);
            
            if (x >= 0 && x < grid.GetLength(0) && y >= 0 && y < grid.GetLength(1))
            {
                // if(grid[x - 1, y - 1]) {return false;};
                if(grid[x, y] != null)
                {
                    return false;
                }
                // else grid[x, y] = true;
            }
            else if(x <= 0)
            {
                return false;
            };
        }
        return true;
    }

    public void LockPlayerBlock(string txt = "")
    {
        TurnOffGhostBlock();
        List<GameObject> listChildBlock = new List<GameObject>();
        isPlayerBlockMoveDown = false;

        int total = PlayerBlock.transform.childCount;
        for(int i = 0; i < 1;)
        {
            total--;
            if(total == 0) i++;
            Transform child = PlayerBlock.transform.GetChild(0);
            Vector3 childPosition = child.position;
            int x = Mathf.RoundToInt((childPosition.y - FirstBlockPosition.y) / blockSize);
            int y = Mathf.RoundToInt((childPosition.x - FirstBlockPosition.x) / blockSize);
            // Debug.Log(x + " / " + y);
            if (grid[x, y] == null && x < grid.GetLength(0)) grid[x, y] = child.gameObject;
            else
            {
                if (grid[x, y]) grid[x, y].transform.localScale = Vector3.one;
                LoseGame();
                return;
            }

            child.position = new Vector3(FirstBlockPosition.x + y * blockSize, FirstBlockPosition.y + x * blockSize, child.position.z);
            ListPlayerBlock.Add(child.gameObject);
            child.SetParent(ListBlock.transform);
            listChildBlock.Add(child.gameObject);
        }

        CheckListBlock(listChildBlock);
    }

    void CheckListBlock(List<GameObject> listChildBlock)
    {
        PlayButterflyEffect = false;
        // if (listChildBlock[0].name == "Rainbow")
        // {
        //     CheckRainbowBlock(listChildBlock[0]);
        //     return;
        // }

        // int time = 0;
        foreach (GameObject childBlock in listChildBlock)
        {
            Vector3 childPosition = childBlock.transform.position;
            int x = Mathf.RoundToInt((childPosition.y - FirstBlockPosition.y) / blockSize);
            int y = Mathf.RoundToInt((childPosition.x - FirstBlockPosition.x) / blockSize);
            // CheckSpecialBlock(x, y);

            // List<GameObject> nearbyBlocks = new List<GameObject>();
            // CheckSameNearbyBlock(nearbyBlocks, childBlock, x, y);
            bool delete = CheckDeleteHorizontal(x);
            // int largestTime = 0;

            if (delete) DeleteHorizontal(x);
            // if(nearbyBlocks.Count > 2) largestTime = DeleteListBlock(nearbyBlocks);
            // if(largestTime > time) time = largestTime;
        }

        // if (time == 0)
        // {
        //     if (PlayButterflyEffect) StartCoroutine(CheckGrid(0.3f));
        //     else WaitForMove();
        // }
        // else StartCoroutine(CheckGrid(time * 0.1f + 0.3f + 0.7f));

        WaitForMove();
    }

    bool CheckDeleteHorizontal(int x)
    {
        for (int j = 0; j < grid.GetLength(1); j++)
        {
            if (grid[x, j] == null) return false;
        }
        return true;
    }

    void DeleteHorizontal(int x)
    {
        for (int i = x; i >= 0; i--)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                if (grid[i, j] == null) continue;
                DeleteBlock(grid[i, j]);
                grid[i, j] = null;
            }
        }
    }

    int DeleteListBlock (List<GameObject> list, string txt = "")
    {
        int time = 0;
        
        int firstX = Mathf.RoundToInt((list[0].transform.position.y - FirstBlockPosition.y) / blockSize);
        int firstY = Mathf.RoundToInt((list[0].transform.position.x - FirstBlockPosition.x) / blockSize);
        
        if (txt == "") gameManager.CheckScore(list[0].name, list.Count);

        int countItem = 0;
        foreach(GameObject child in list)
        {
            if(DOTween.IsTweening(child)) break;
            if(child.name == "Rock") continue;
            int x = Mathf.RoundToInt((child.transform.position.y - FirstBlockPosition.y) / blockSize);
            int y = Mathf.RoundToInt((child.transform.position.x - FirstBlockPosition.x) / blockSize);
            int childTimer = Mathf.Abs(firstX - x) + Mathf.Abs(firstY - y);
            if(childTimer > time) time = childTimer;
    
            if (txt == "tool") gameManager.CheckScore(child.name, 1);
            child.transform.DOScale(child.transform.localScale * 1.2f, 0.1f).SetEase(Ease.OutBounce).SetDelay(0.1f * childTimer).OnComplete(() => {
                // audioManager.PlaySFX("EatBlock");
                child.transform.DOScale(Vector3.zero, 0.2f).OnComplete(() => {
                    PlayEffectEatBlock(child);
                    blockTextureMap.Remove(child);
                    DeleteBlock(child);
                });
            });
            grid[x, y] = null;

            countItem++;
            if(countItem > 3)
            {
                PlayStarAnimation(child.transform.position, 0.1f * childTimer + 0.1f);
            }

        }
        return time;
    }

    void PlayEffectEatBlock(GameObject child)
    {
        // GameObject effect = ObjectPoolManager.SpawnObject(EffectEatBlockPrefab, Vector3.zero, Quaternion.identity);
        // effect.name = "EffectEatBlock";
        // effect.transform.SetParent(ListEffectEatBlock.transform);
        // effect.transform.localScale = Vector3.one;

        // effect.transform.position = child.transform.position;
        // SkeletonGraphic skeletonGraphic = effect.GetComponent<SkeletonGraphic>();
        // skeletonGraphic.Skeleton.SetSkin($"Theme{CurrentTheme}_{child.name}");
        // skeletonGraphic.AnimationState.SetAnimation(0, "animation", false);
        
        // StartCoroutine(DisableEffectEatBlock(effect));
    }

    IEnumerator DisableEffectEatBlock(GameObject effect)
    {
        yield return new WaitForSeconds(0.7f);
        // ObjectPoolManager.ReturnObjectToPool(effect);
        // effect.transform.SetParent(EffectPool.transform);
    }

    bool isPlayerBlockMoveDown = false;

    IEnumerator CheckGrid(float time, string txt = null)
    {
        int timer = 0;
        yield return new WaitForSeconds(time);
        int iLength = grid.GetLength(0);
        int jLength = grid.GetLength(1);
        for (int i = 0; i < iLength; i++)
        {
            for (int j = 0; j < jLength; j++)
            {
                GameObject block = grid[i, j];
                if (block != null)
                {
                    if (i - 1 >= 0 && grid[i - 1, j] == null)
                    {
                        List<GameObject> nearbyBlocks = new List<GameObject>();
                        CheckNearbyBlock(nearbyBlocks, block.transform, i, j);
                        GameObject checkBlock = nearbyBlocks.Find(block => Mathf.RoundToInt((block.transform.position.y - FirstBlockPosition.y) / blockSize) == 0);

                        if (checkBlock == null)
                        {
                            int endTime = MoveBlockToBottom(nearbyBlocks);
                            // int endTime1 = StartCoroutine(test());
                            if (endTime > timer && endTime != iLength) timer = endTime;
                        }
                    }
                }
            }
        }
        yield return new WaitForSeconds(0.1f);
        if (isPlayerBlockMoveDown)
        {
            Invoke("CheckPlayerBlockAgain", 0.1f * (float)timer);
            if (txt == "tool") Invoke("DisableTouchInSupportTools", 0.1f * (float)timer);
        }
        else if (txt == null) Invoke("WaitForMove", 0.1f * (float)timer);
        else if (txt == "tool") Invoke("CheckMoveBlocksInTheSpecifiedDirection", 0.1f * (float)timer);
    }

    //  IEnumerator test()
    // {
    //     yield return new WaitForSeconds(1);
    //     return true;
    // }

    void CheckPlayerBlockAgain()
    {
        List<GameObject> listChildBlock = ListPlayerBlock;
        isPlayerBlockMoveDown = false;
        CheckListBlock(listChildBlock);
    }

    void CheckMoveBlocksInTheSpecifiedDirection()
    {
        int timer = MoveBlocksInTheSpecifiedDirection("tool");
        Invoke("DisableTouchInSupportTools", 0.1f * timer);
        // supportTools.DisableTouch();
    }

    void DisableTouchInSupportTools()
    {
        // supportTools.DisableTouch();
        SetGhostBLockPosition();
        CheckWinGame();
    }

    int MoveBlockToBottom(List<GameObject> nearbyBlocks)
    {
        int gridHeight = grid.GetLength(0);
        int shortest = gridHeight;
        foreach(GameObject child in nearbyBlocks)
        {
            if(DOTween.IsTweening(child.transform)) break;
            if (child.name == "Rock") { return gridHeight; } // Không thể di chuyển xuống nữa vì gặp đá

            int x = Mathf.RoundToInt((child.transform.position.y - FirstBlockPosition.y) / blockSize);
            int y = Mathf.RoundToInt((child.transform.position.x - FirstBlockPosition.x) / blockSize);
            int count = 0;
            for(int i = x - 1; i >= 0; i--)
            {
                if (grid[i, y] == null) count++;
                else if (nearbyBlocks.Contains(grid[i, y]))
                {
                    count = gridHeight;
                    break;
                }
                else break;
            }

            if(shortest > count) shortest = count;
        }

        if(shortest < gridHeight)
        {
            foreach (GameObject child in nearbyBlocks)
            {
                int x = Mathf.RoundToInt((child.transform.position.y - FirstBlockPosition.y) / blockSize);
                int y = Mathf.RoundToInt((child.transform.position.x - FirstBlockPosition.x) / blockSize);
                grid[x, y] = null;
                if (!isPlayerBlockMoveDown && ListPlayerBlock.Contains(child)) isPlayerBlockMoveDown = true;
                int distance = shortest;
                string childName = child.name;
                child.name = "falling";
                child.transform.DOMove(new Vector3(child.transform.position.x, FirstBlockPosition.y + (x - distance) * blockSize, child.transform.position.z), 0.1f * distance)
                    .SetEase(Ease.InCubic).OnComplete(() => child.name = childName );
            }

            foreach(GameObject child in nearbyBlocks)
            {
                int x = Mathf.RoundToInt((child.transform.position.y - FirstBlockPosition.y) / blockSize);
                int y = Mathf.RoundToInt((child.transform.position.x - FirstBlockPosition.x) / blockSize);
                // grid[x, y] = null;
                grid[x - shortest, y] = child;
            }
        }

        return shortest;
    }

    void WaitForMove()
    {
        if (!isWinGame) CheckWinGame();

        MoveBlocksInTheSpecifiedDirection();
        // int time = MoveBlocksInTheSpecifiedDirection();
    }

    void CheckWinGame()
    {
        isWinGame = gameManager.CheckWinGame();
    }
    
    public void CheckNearbyBlock(List<GameObject> nearbyBlocks, Transform block, int x, int y)
    {
        if (nearbyBlocks.Contains(block.gameObject)) return;
        nearbyBlocks.Add(block.gameObject);

        // Tạo các hướng: (dx, dy)
        int[,] directions = new int[,]
        {
            { 1, 0 },  // phải
            {-1, 0 },  // trái
            { 0, 1 },  // trên
            { 0,-1 }   // dưới
        };

        for (int i = 0; i < directions.GetLength(0); i++)
        {
            int nx = x + directions[i, 0];
            int ny = y + directions[i, 1];

            if (nx >= 0 && nx < grid.GetLength(0) && ny >= 0 && ny < grid.GetLength(1))
            {
                GameObject neighbor = grid[nx, ny];
                if (neighbor != null && neighbor.name != "falling")
                {
                    CheckNearbyBlock(nearbyBlocks, neighbor.transform, nx, ny);
                }
            }
        }
    }
    
    public void CheckSameNearbyBlock(List<GameObject> nearbyBlocks, GameObject block, int x, int y, bool isFutureCheck = false)
    {
        if (nearbyBlocks.Contains(block)) return;
        nearbyBlocks.Add(block);

        string blockName = block.name;

        int[,] directions = new int[,]
        {
            { 1, 0 },  // phải
            {-1, 0 },  // trái
            { 0, 1 },  // trên
            { 0,-1 }   // dưới
        };

        for (int i = 0; i < directions.GetLength(0); i++)
        {
            int nx = x + directions[i, 0];
            int ny = y + directions[i, 1];

            if (nx >= 0 && nx < grid.GetLength(0) && ny >= 0 && ny < grid.GetLength(1))
            {
                GameObject neighbor = grid[nx, ny];
                if (neighbor != null && neighbor.name == blockName)
                {
                    if (!isFutureCheck ||
                        (isFutureCheck && (!blockTextureMap.TryGetValue(neighbor, out string textureName) || blockTextureMap[neighbor] != "Question")))
                        CheckSameNearbyBlock(nearbyBlocks, neighbor, nx, ny, isFutureCheck);
                }
            }
        }
    }

    public bool CheckBlockTouch(float nextPositionX, Transform block, float nextBlockPosition)
    {
        int length = block.childCount;
        float firstX = FirstBlockPosition.x;
        float firstY = FirstBlockPosition.y;

        int width = grid.GetLength(0);
        bool isRight = nextPositionX > 0;
        int offsetY = isRight ? 1 : -1;

        for (int i = 0; i < length; i++)
        {
            Vector3 childPosition = block.GetChild(i).position;
            int x = Mathf.CeilToInt((childPosition.y - firstY) / blockSize);
            x = x == -1 ? 0 : x;
            int xNext = Mathf.CeilToInt((childPosition.y + nextBlockPosition - firstY) / blockSize);
            int y = Mathf.RoundToInt((childPosition.x - firstX) / blockSize);
            int targetY = y + offsetY;


            if (grid[x, targetY] != null) return false;

            if (x <= xNext)
            {
                if (x + 1 < width && grid[x + 1, targetY] != null) return false;
            }

            // if (x > xNext)
            // {
            //     if (isRight)
            //     {
            //         if (grid[x, targetY] != null) return false;
            //     }
            //     else
            //     {
            //         if (grid[x, targetY] != null) return false;
            //     }
            // }
            // else
            // {
            //     if (isRight)
            //     {
            //         if (grid[x, targetY] != null) return false;
            //         if (x + 1 < width && grid[x + 1, targetY] != null) return false;
            //     }
            //     else
            //     {
            //         if (grid[x, targetY] != null) return false;
            //         if (x + 1 < width && grid[x + 1, targetY] != null) return false;
            //     }
            // }
        }
        return true;
    }

    public bool CheckBlockTouchWhenRotate(List<Vector3> listNewPosition, Transform block, float nextBlockMoveY)
    {
        bool touchLeft = false;
        bool touchRight = false;
        bool touchTop = false;
        bool touchBottom = false;
        
        float hightestBlock = 0;
        int width = grid.GetLength(0);
        int height = grid.GetLength(1);

        for (int i = 0; i < listNewPosition.Count; i++)
        {
            hightestBlock = listNewPosition[i].y > hightestBlock ? listNewPosition[i].y : hightestBlock;
            Vector3 childlocalPosition = listNewPosition[i];
            Vector3 childPosition = childlocalPosition + block.position;
            int x = Mathf.CeilToInt((childPosition.y - FirstBlockPosition.y) / blockSize);
            int xNext = Mathf.CeilToInt((childPosition.y + nextBlockMoveY - FirstBlockPosition.y) / blockSize);
            int y = Mathf.RoundToInt((childPosition.x - FirstBlockPosition.x) / blockSize);

            if (x >= 0 && x < width && y >= 0 && y < height)
            {
                if (grid[x, y] != null)
                {
                    if (Mathf.Round(childlocalPosition.x) > 0) touchRight = true;
                    if (Mathf.Round(childlocalPosition.x) < 0) touchLeft = true;
                    if (Mathf.Round(childlocalPosition.y) > 0) touchTop = true;
                    if (Mathf.Round(childlocalPosition.y) < 0) touchBottom = true;
                    // return false;
                }

                if (x <= xNext && x + 1 < width)
                {
                    if (grid[x + 1, y] != null)
                    {
                        if (Mathf.Round(childlocalPosition.x) > 0) touchRight = true;
                        if (Mathf.Round(childlocalPosition.x) < 0) touchLeft = true;
                        if (Mathf.Round(childlocalPosition.y) > 0) touchTop = true;
                        if (Mathf.Round(childlocalPosition.y) < 0) touchBottom = true;
                    }
                }
            }
            else
            {
                if (x < 0) touchBottom = true;
                if (x >= width) touchTop = true;
                if (y < 0) touchLeft = true;
                if (y >= height) touchRight = true;
                // return false;
            }
        }
        
        hightestBlock += block.position.y;
        float limitedHeight = -FirstBlockPosition.y + MapFramePosition.y * 2;
        if (limitedHeight < hightestBlock && !touchBottom)
        {
            block.position -= new Vector3(0, hightestBlock - limitedHeight, 0);
            return true;
        }

        if ((touchLeft && touchRight) || (touchBottom && touchTop)) return false;
        string status = "";
        if (touchLeft)
        {
            if (touchTop) status = "topLeft";
            else if (touchBottom) status = "bottomLeft";
            else status = "left";
        }
        else if (touchRight)
        {
            if (touchTop) status = "topRight";
            else if (touchBottom) status = "bottomRight";
            else status = "right";
        }
        else if (touchTop) status = "top";
        else if (touchBottom) status = "bottom";
        else return true;

        return IsValidToRotate(listNewPosition, block, status);
    }

    bool IsValidToRotate (List<Vector3> listNewPosition, Transform block, string txt)
    {
        float xx = 0;
        float yy = 0;

        switch (txt)
        {
            case "left":
                xx = blockSize;
                break;
            case "right":
                xx = -blockSize;
                break;
            case "top":
                yy = -blockSize;
                break;
            case "bottom":
                yy = blockSize;
                break;
            case "topLeft":
                xx = blockSize;
                yy = -blockSize;
                break;
            case "topRight":
                xx = -blockSize;
                yy = -blockSize;
                break;
            case "bottomLeft":
                xx = blockSize;
                yy = blockSize;
                break;
            case "bottomRight":
                xx = -blockSize;
                yy = blockSize;
                break;
        }
        // Debug.Log(txt);
        // if (txt == "top")
        // {
        //     float hightestBlock = 0;
        //     for (int i = 0; i < listNewPosition.Count; i++)
        //     {
        //         hightestBlock = listNewPosition[i].y > hightestBlock ? listNewPosition[i].y : hightestBlock;
        //     }
        //     hightestBlock += block.position.y;
        //     float limitedHeight = -FirstBlockPosition.y + MapFrame.transform.position.y * 2;
        //     if (limitedHeight < hightestBlock)
        //     {
        //         block.position -= new Vector3(0, hightestBlock - limitedHeight, 0);
        //         return true;
        //     }
        // }

        if (xx != 0 || yy != 0)
            {
                if (!CheckTouchAfterMoveBlock(listNewPosition, block, new Vector3(xx, 0, 0)))
                {
                    if (!CheckTouchAfterMoveBlock(listNewPosition, block, new Vector3(0, yy, 0)))
                    {
                        if (!CheckTouchAfterMoveBlock(listNewPosition, block, new Vector3(xx, yy, 0)))
                        {
                            /////////////////////////
                            if (block.name == "I")
                            {
                                float child0X = block.GetChild(0).localPosition.x;
                                if (((txt == "left" || txt == "topLeft" || txt == "bottomLeft") && child0X > 0) ||
                                    ((txt == "right" || txt == "topRight" || txt == "bottomRight") && child0X < 0)
                                    // ||(txt == "top")
                                    )
                                {
                                    xx *= 2;
                                    yy *= 2;
                                }

                                if (!CheckTouchAfterMoveBlock(listNewPosition, block, new Vector3(xx, 0, 0)))
                                {
                                    if (!CheckTouchAfterMoveBlock(listNewPosition, block, new Vector3(0, yy, 0)))
                                    {
                                        if (!CheckTouchAfterMoveBlock(listNewPosition, block, new Vector3(xx, yy, 0)))
                                        {
                                            return false;
                                        }
                                        else block.position += new Vector3(xx, yy, 0);
                                    }
                                    else block.position += new Vector3(0, yy, 0);
                                }
                                else block.position += new Vector3(xx, 0, 0);
                            }
                            else return false;
                            /////////////////////
                        }
                        else block.position += new Vector3(xx, yy, 0);
                    }
                    else block.position += new Vector3(0, yy, 0);
                }
                else block.position += new Vector3(xx, 0, 0);
            }
        // else
        // {
        //     if(CheckTouchAfterMoveBlock(listNewPosition, block, new Vector3(xx, yy, 0)))
        //         block.position += new Vector3(xx, yy, 0);
        //     else return false;
        // }

        return true;
    }

    bool CheckTouchAfterMoveBlock (List<Vector3> listNewPosition, Transform block, Vector3 pos)
    {
        for (int i = 0; i < listNewPosition.Count; i++)
        {
            Vector3 childPosition = listNewPosition[i] + block.position + pos;
            int x = Mathf.CeilToInt((childPosition.y - FirstBlockPosition.y) / blockSize);
            int y = Mathf.RoundToInt((childPosition.x - FirstBlockPosition.x) / blockSize);

            if (x >= 0 && x < grid.GetLength(0) && y >= 0 && y < grid.GetLength(1))
            {
                if (grid[x, y] != null) return false;
            }
            else if (x >= grid.GetLength(0))
            {
                return false;
            }
            else return false;
        }
        return true;
    }

    public float CheckNextPosition(float distance)
    {
        float minDistance = distance;
        int PlayCount = PlayerBlock.transform.childCount;
        
        int gridHeight = grid.GetLength(0) - 1;
        for (int a = 0; a < PlayCount; a++)
        {
            Vector3 childPosition = PlayerBlock.transform.GetChild(a).position;
            int currentx = Mathf.CeilToInt((childPosition.y - FirstBlockPosition.y) / blockSize);
            int nextx = Mathf.CeilToInt((childPosition.y - FirstBlockPosition.y + distance) / blockSize);
            int y = Mathf.RoundToInt((childPosition.x - FirstBlockPosition.x) / blockSize);

            for (int i = currentx; i <= nextx; i++)
            {
                if (i <= gridHeight && grid[i, y] != null)
                {
                    // Tính khoảng cách đến collision
                    float distanceToCollision = FirstBlockPosition.y + (blockSize * (i - 1)) - childPosition.y;
                    minDistance = minDistance > distanceToCollision ? distanceToCollision : minDistance;
                    break;
                }
                else if (i >= gridHeight)
                {
                    float distanceToCollision = FirstBlockPosition.y - childPosition.y;
                    minDistance = minDistance > distanceToCollision ? distanceToCollision : minDistance;
                    break;
                }
                // }
            }
        }

        return minDistance;
    }

    int MoveBlocksInTheSpecifiedDirection(string txt = "")
    {
        int time = 0;
        if(MapDirection == "")
        {
            if (txt == "") CreateRandomBlock();
            return time;
        }
        int iLength = grid.GetLength(0);
        int jLength = grid.GetLength(1);
        if (MapDirection == "right")
        {
            for (int i = 0; i < iLength; i++)
            {
                int countEmptyBlock = 0;
                for (int j = jLength - 1; j >= 0; j--)
                {
                    GameObject block = grid[i, j];
                    if (block == null) { countEmptyBlock++; continue; }
                    if (countEmptyBlock > 0)
                    {
                        time = time < countEmptyBlock ? countEmptyBlock : time;
                        block.transform.DOLocalMove(new Vector3(FirstBlockPosition.x + (j + countEmptyBlock) * blockSize, block.transform.position.y, block.transform.position.z), 0.1f * countEmptyBlock);
                        grid[i, j + countEmptyBlock] = block;
                        grid[i, j] = null;
                    }
                }
            }
        }
        else if (MapDirection == "left")
        {
            for (int i = 0; i < iLength; i++)
            {
                int countEmptyBlock = 0;
                for (int j = 0; j < jLength; j++)
                {
                    GameObject block = grid[i, j];
                    if (block == null) { countEmptyBlock++; continue; }
                    if (countEmptyBlock > 0)
                    {
                        time = time < countEmptyBlock ? countEmptyBlock : time;
                        block.transform.DOLocalMove(new Vector3(FirstBlockPosition.x + (j - countEmptyBlock) * blockSize, block.transform.position.y, block.transform.position.z), 0.1f * countEmptyBlock);
                        grid[i, j - countEmptyBlock] = block;
                        grid[i, j] = null;
                    }
                }
            }
        }

        if (txt == "") Invoke("CheckListPlayerBlock", 0.1f * time);

        return time;
    }

    int ListPlayerBlockLength = 10;
    void CheckListPlayerBlock()
    {
        int listPlayerBlockLength = 0;
        if (ListPlayerBlockLength == 10)
        {
            foreach (GameObject child in ListPlayerBlock)
            {
                if (child.transform.parent.name == "ListBlock")
                {
                    listPlayerBlockLength++;
                }
            }
        }

        if (listPlayerBlockLength > 0 && listPlayerBlockLength < ListPlayerBlockLength)
        {
            ListPlayerBlockLength = listPlayerBlockLength;
            CheckPlayerBlockAgain();
            return;
        }

        ListPlayerBlockLength = 10;
        CreateRandomBlock();
    }

    public void MovePlayerBlock()
    {
        int playerBlockLength = PlayerBlock.transform.childCount;
        if (playerBlockLength == 0) return;
        int blockHeight = 0;
        int gridHeight = grid.GetLength(0);
        for (int z = 0; z < playerBlockLength; z++)
        {
            Vector3 blockPosition = PlayerBlock.transform.GetChild(z).position;
            int originalBlocki = Mathf.RoundToInt((blockPosition.y - FirstBlockPosition.y) / blockSize);
            int j = Mathf.RoundToInt((blockPosition.x - FirstBlockPosition.x) / blockSize);
            // Debug.Log(originalBlocki + " / " + j);
            for (int i = 0; i < gridHeight; i++)
            {
                GameObject child = grid[i, j];
                if (child != null)
                {
                    int height = originalBlocki + (i - 1);
                    blockHeight = blockHeight < height ? height : blockHeight;
                    break;
                }
                else if (i == gridHeight - 1 && child == null)
                {
                    int height = originalBlocki + i;
                    blockHeight = blockHeight < height ? height : blockHeight;
                    break;
                }
            }
        }

        // Debug.Log(blockHeight);
        for (int z = 0; z < playerBlockLength; z++)
        {
            Vector3 blockPosition = PlayerBlock.transform.GetChild(z).position;

            int i = Mathf.RoundToInt((blockPosition.y - FirstBlockPosition.y) / blockSize);
            int j = Mathf.RoundToInt((blockPosition.x - FirstBlockPosition.x) / blockSize);

            Transform child = GhostBlock.transform.GetChild(z);
            child.position = FirstBlockPosition + new Vector3(j * blockSize, (i + blockHeight) * blockSize, 0.2f);
        }
    }
    
#endregion

#region Ghost Block manager

    void SetGhostBlockImage(GameObject block, int CurrentTheme, int randomTexture)
    {
        Renderer renderer = block.GetComponent<Renderer>();
        material.Clear();

        renderer.sharedMaterial = transparentMaterial;
        Color baseColor = Color.white;
        baseColor.a = 0.3f;
        material.SetColor("_BaseColor", baseColor);

        textureResources.SetTexture(material, $"{CurrentTheme}.{randomTexture}");
        renderer.SetPropertyBlock(material);
    }

    void CreateListGhostBlock()
    {
        for (int i = 0; i < 4; i++)
        {
            GameObject ghostBLock = ObjectPoolManager.SpawnObject(blockPrefab, Vector3.zero, Quaternion.identity);
            ghostBLock.SetActive(false);
            ghostBLock.transform.SetParent(GhostBlock.transform);
            ghostBLock.transform.localScale = new Vector3(blockScale, blockScale, blockScale);
        }
    }

    void CreateGhostOfPlayerBlock(string txt ="")
    {
        // int PlayerBlockLength = PlayerBlock.transform.childCount;
        // // CurrentTheme = (int)saveDataJson.GetData("CurrentTheme");
        // CurrentTheme = 1;

        // for (int i = 0; i < PlayerBlockLength; i++)
        // {
        //     string name = PlayerBlock.transform.GetChild(i).name;
        //     GameObject ghostBLock = GhostBlock.transform.GetChild(i).gameObject;
        //     ghostBLock.SetActive(true);
        //     ghostBLock.name = name;
        //     if (txt == "Rainbow") SetBlockImage(ghostBLock, txt);
        //     else SetGhostBlockImage(ghostBLock, CurrentTheme, int.Parse(name));
        // }
        // SetGhostBLockPosition();
    }

    public void SetGhostBLockPosition()
    {
        StopActionListBlockGonnaBeDestroy();
        int playerBlockLength = PlayerBlock.transform.childCount;
        if (playerBlockLength == 0) return;
        int blockHeight = grid.GetLength(0);
        for (int z = 0; z < playerBlockLength; z++)
        {
            Vector3 originalBlockPosition = PlayerBlock.transform.GetChild(z).position;
            int originalBlocki = Mathf.RoundToInt((originalBlockPosition.y - FirstBlockPosition.y) / blockSize);
            int j = Mathf.RoundToInt((originalBlockPosition.x - FirstBlockPosition.x) / blockSize);

            for (int i = 0; i < grid.GetLength(0); i++)
            {
                GameObject child = grid[i, j];
                if (child != null)
                {
                    int height = originalBlocki - (i - 1);
                    blockHeight = blockHeight > height ? height : blockHeight;
                    break;
                }
                else if (i == grid.GetLength(0) && child == null)
                {
                    int height = originalBlocki - i;
                    blockHeight = blockHeight > height ? height : blockHeight;
                    break;
                }
            }
        }

        for (int z = 0; z < playerBlockLength; z++)
        {
            Vector3 originalBlockPosition = PlayerBlock.transform.GetChild(z).position;

            int i = Mathf.RoundToInt((originalBlockPosition.y - FirstBlockPosition.y) / blockSize);
            int j = Mathf.RoundToInt((originalBlockPosition.x - FirstBlockPosition.x) / blockSize);

            Transform child = GhostBlock.transform.GetChild(z);
            child.position = FirstBlockPosition + new Vector3(j * blockSize, (i - blockHeight) * blockSize, 0.2f);
        }
        CheckListBlockCanBeDestroy();
    }
    
    void CheckListBlockCanBeDestroy()
    {
        int playerBlockLength = PlayerBlock.transform.childCount;
        for (int z = 0; z < playerBlockLength; z++)
        {
            Transform child = GhostBlock.transform.GetChild(z);
            int i = Mathf.RoundToInt((child.position.y - FirstBlockPosition.y) / blockSize);
            int j = Mathf.RoundToInt((child.position.x - FirstBlockPosition.x) / blockSize);

            List<GameObject> nearbyBlocks = new List<GameObject>();
            CheckSameNearbyBlock(nearbyBlocks, child.gameObject, i, j, true);
            CheckSameNearInGhostBlock(nearbyBlocks, child.gameObject, child.name);
            if (nearbyBlocks.Count > 2) DoActionListBlockGonnaBeDestroy(nearbyBlocks);
        }
    }

    void CheckSameNearInGhostBlock(List<GameObject> nearbyBlocks, GameObject block, string name)
    {
        Vector3 blockPosition = block.transform.position;

        int playerBlockLength = PlayerBlock.transform.childCount;
        for (int i = 0; i < playerBlockLength; i++)
        {
            Transform child = GhostBlock.transform.GetChild(i);
            if(nearbyBlocks.Contains(child.gameObject)) continue;
            Vector3 childPosition = child.position;
            float distance = Vector3.Distance(blockPosition, childPosition);
            if (child.name == name)
            {
                int x = Mathf.RoundToInt((childPosition.y - FirstBlockPosition.y) / blockSize);
                int y = Mathf.RoundToInt((childPosition.x - FirstBlockPosition.x) / blockSize);
                if (distance < blockSize * 1.5f && distance > 0 &&
                    (childPosition.x == blockPosition.x || childPosition.y == blockPosition.y))
                // if (childPosition.x == blockPosition.x || childPosition.y == blockPosition.y)
                {
                    CheckSameNearbyBlock(nearbyBlocks, child.gameObject, x, y, true);
                    CheckSameNearInGhostBlock(nearbyBlocks, child.gameObject, name);
                }
                else
                {
                    for (int j = 0; j < nearbyBlocks.Count; j++)
                    {
                        int a = Mathf.RoundToInt((nearbyBlocks[j].transform.position.y - FirstBlockPosition.y) / blockSize);
                        int b = Mathf.RoundToInt((nearbyBlocks[j].transform.position.x - FirstBlockPosition.x) / blockSize);
                        if (a == x && Mathf.Abs(b - y) == 1 || b == y && Mathf.Abs(a - x) == 1)
                        {
                            CheckSameNearbyBlock(nearbyBlocks, child.gameObject, x, y, true);
                            CheckSameNearInGhostBlock(nearbyBlocks, child.gameObject, name);
                        }
                    }
                }
            }
        }
    }
    List<GameObject> ListHighlightBlock = new List<GameObject>();
    void DoActionListBlockGonnaBeDestroy(List<GameObject> nearbyBlocks)
    {
        Color color = new Color(0.89f, 0.937f, 0.31f);
        foreach (GameObject child in nearbyBlocks)
        {
            Renderer renderer = child.GetComponent<Renderer>();
            ListHighlightBlock.Add(child);

            // MaterialPropertyBlock material = new MaterialPropertyBlock();
            material.Clear();
            renderer.GetPropertyBlock(material);
            if (material.GetColor("_BaseColor").r == color.r) return;

            if (child.transform.parent.name == "GhostBlock") color.a = 0.3f;
            material.SetColor("_BaseColor", color);
            renderer.SetPropertyBlock(material);
                // renderer.material.color = color;

            // Outline outline = child.GetComponent<Outline>();
            // outline.enabled = true;
            // outline.OutlineColor = color;
            // outline.OutlineMode = Outline.Mode.OutlineVisible;
            // outline.OutlineWidth = 3f;
        }
    }

    void StopActionListBlockGonnaBeDestroy()
    {
        if (ListHighlightBlock.Count == 0) return;
        Color color = Color.white;
        foreach (GameObject child in ListHighlightBlock)
        {
            if (child == null) continue;
            Renderer renderer = child.GetComponent<Renderer>();
            // MaterialPropertyBlock material = new MaterialPropertyBlock();
            material.Clear();
            renderer.GetPropertyBlock(material);
            if (child.transform.parent.name == "GhostBlock") color.a = 0.3f;
            material.SetColor("_BaseColor", color);
            renderer.SetPropertyBlock(material);
        }
        ListHighlightBlock.Clear();
    }

    void TurnOffGhostBlock()
    {
        if (GhostBlock.transform.childCount == 0) CreateListGhostBlock();
        else
        {
            foreach (Transform child in GhostBlock.transform)
                {
                    child.gameObject.SetActive(false);
                }
            StopActionListBlockGonnaBeDestroy();
        }
    }
#endregion Ghost block manager

#region Tool
    // public List<GameObject> ListBlockInfluence = new List<GameObject>();
    // // GameObject firstBlock;
    // public void CheckListBlockToDestroy(string ToolType, Vector2 CurrentTouch)
    // {
    //     int x = Mathf.RoundToInt((CurrentTouch.y - FirstBlockPosition.y) / blockSize);
    //     int y = Mathf.RoundToInt((CurrentTouch.x - FirstBlockPosition.x) / blockSize);
    //     if (x < 0 || x >= 16 || y < 0 || y >= 9)
    //     {
    //         RedoListBlockInfluence();
    //         ListBlockInfluence.Clear();
    //         return;
    //     }

    //     if (ListBlockInfluence.Count > 0)
    //     {
    //         if (grid[x, y] == ListBlockInfluence[0]) return;
    //         RedoListBlockInfluence();
    //     }
        
    //     ListBlockInfluence.Clear();

    //     if (grid[x, y] != null) ListBlockInfluence.Add(grid[x, y]);

    //     GameObject child;
    //     switch (ToolType)
    //     {
    //         case "Boom":
    //             for (int i = x - 1; i <= x + 1; i++)
    //             {
    //                 for (int j = y - 1; j <= y + 1; j++)
    //                 {
    //                     if (i < 0 || i >= 16 || j < 0 || j >= 9) continue;
    //                     child = grid[i, j];
    //                     if (child == null) continue;
    //                     if (ListBlockInfluence.Count > 0 && child == ListBlockInfluence[0]) continue;
    //                     ListBlockInfluence.Add(child);
    //                 }
    //             }
    //             break;
    //         case "TNT":
    //             for (int i = x + 1; i < 16; i++)
    //             {
    //                 child = grid[i, y];
    //                 if (child == null) continue;
    //                 ListBlockInfluence.Add(child);
    //             }

    //             for (int i = x - 1; i >= 0; i--)
    //             {
    //                 child = grid[i, y];
    //                 if (child == null) continue;
    //                 ListBlockInfluence.Add(child);
    //             }
    //             break;
    //         case "Hammer":
    //             for (int j = y + 1; j < 9; j++)
    //             {
    //                 child = grid[x, j];
    //                 if (child == null) continue;
    //                 ListBlockInfluence.Add(child);
    //             }

    //             for (int j = y - 1; j >= 0; j--)
    //             {
    //                 child = grid[x, j];
    //                 if (child == null) continue;
    //                 ListBlockInfluence.Add(child);
    //             }
    //             break;
    //     }

    //     // StartCoroutine(PlayAnimListBlockInfluence(ListBlockInfluence));
    //     ShowListBlockInfluence();
    // }

    // public void RedoListBlockInfluence()
    // {
    //     foreach (GameObject child in ListBlockInfluence)
    //     {
    //         child.GetComponent<Renderer>().material.color = Color.white;
    //     }
    // }
    
    // void ShowListBlockInfluence()
    // {
    //     foreach (GameObject child in ListBlockInfluence)
    //     {
    //         child.GetComponent<Renderer>().material.color = new Color(0.5f, 0.5f, 0.5f, 1f);
    //     }
    // }

    // public bool ActiveBoom()
    // {
    //     if (ListBlockInfluence.Count == 0) return false;
    //     RedoListBlockInfluence();
    //     int timer = DeleteListBlock(ListBlockInfluence, "tool");
    //     ListBlockInfluence.Clear();

    //     StartCoroutine(CheckGrid(0.3f + 0.1f * timer, "tool"));
    //     return true;
    // }

    // public bool DeleteHorizontalRow()
    // {
    //     // xoá hàng ngang
    //     if (ListBlockInfluence.Count == 0) return false;

    //     RedoListBlockInfluence();
    //     int x = Mathf.RoundToInt((ListBlockInfluence[0].transform.position.y - FirstBlockPosition.y) / blockSize);
    //     int timer = DropHorizontalRowAnimated(x);
    //     // int timer = DeleteListBlockByDirection(ListBlockInfluence, "vertical");
    //     ListBlockInfluence.Clear();

    //     // StartCoroutine(CheckGrid(0.3f + 0.1f * timer, "tool"));
    //     StartCoroutine(CheckGrid(0.2f + 0.1f * timer, "tool"));
    //     return true;
    // }

    // public bool DeleteVerticalRow()
    // {
    //     // xoá hàng dọc
    //     if (ListBlockInfluence.Count == 0) return false;

    //     RedoListBlockInfluence();
    //     // int timer = DeleteListBlockByDirection(ListBlockInfluence, "horizontal");
    //     // ListBlockInfluence.Clear();

    //     // StartCoroutine(CheckGrid(0.3f + 0.1f * timer, "tool"));

    //     int y = Mathf.RoundToInt((ListBlockInfluence[0].transform.position.x - FirstBlockPosition.x) / blockSize);
    //     int timer = DropDeleteVerticalRowAnimated(y);
    //     ListBlockInfluence.Clear();
    //     StartCoroutine(CheckGrid(0.2f + 0.1f * timer, "tool"));
    //     return true;
    // }

    // void CheckRainbowBlock(GameObject block)
    // {
    //     Vector3 blockPosition = block.transform.position;
    //     int x = Mathf.RoundToInt((blockPosition.y - FirstBlockPosition.y) / blockSize);
    //     int y = Mathf.RoundToInt((blockPosition.x - FirstBlockPosition.x) / blockSize);
    //     List<GameObject> ListnearbyBlock1 = new List<GameObject>();
    //     List<GameObject> ListnearbyBlock2 = new List<GameObject>();
    //     List<GameObject> ListnearbyBlock3 = new List<GameObject>();
    //     List<GameObject> ListnearbyBlock4 = new List<GameObject>();
    //     List<GameObject> ListnearbyBlock = new List<GameObject>();
    //     int timer = 0;
    //     int count = 0;

    //     if(x + 1 < grid.GetLength(0) && grid[x + 1, y] != null) 
    //     {
    //         // ListnearbyBlock1.Add(block);
    //         CheckSameNearbyBlock(ListnearbyBlock1, grid[x + 1, y], x + 1, y);
    //         if(ListnearbyBlock1.Count > 1) count = DeleteListBlock(ListnearbyBlock1);
    //         else ListnearbyBlock.Add(grid[x + 1, y]);
    //         timer = count > timer ? count : timer;
    //     }
    //     if(x - 1 >= 0 && grid[x - 1, y] != null) 
    //     {
    //         // ListnearbyBlock2.Add(block);
    //         CheckSameNearbyBlock(ListnearbyBlock2, grid[x - 1, y], x - 1, y);
    //         if(ListnearbyBlock2.Count > 1) count = DeleteListBlock(ListnearbyBlock2);
    //         else ListnearbyBlock.Add(grid[x - 1, y]);
    //         timer = count > timer ? count : timer;
    //     }
    //     if(y + 1 < grid.GetLength(1) && grid[x, y + 1] != null) 
    //     {
    //         // ListnearbyBlock3.Add(block);
    //         CheckSameNearbyBlock(ListnearbyBlock3, grid[x, y + 1], x, y + 1);
    //         if(ListnearbyBlock3.Count > 1) count = DeleteListBlock(ListnearbyBlock3);
    //         else ListnearbyBlock.Add(grid[x, y + 1]);
    //         timer = count > timer ? count : timer;
    //     }
    //     if(y - 1 >= 0 && grid[x, y - 1] != null) 
    //     {
    //         // ListnearbyBlock4.Add(block);
    //         CheckSameNearbyBlock(ListnearbyBlock4, grid[x, y - 1], x, y - 1);
    //         if(ListnearbyBlock4.Count > 1) count = DeleteListBlock(ListnearbyBlock4);
    //         else ListnearbyBlock.Add(grid[x, y - 1]);
    //         timer = count > timer ? count : timer;
    //     }

    //     if(ListnearbyBlock.Count > 1)
    //     {
    //         count = CountUsingLINQ(ListnearbyBlock);
    //         timer = count > timer ? count : timer;
    //     }

    //     // supportTools.DisableTouch();

    //     if (timer > 0)
    //     {
    //         timer++;
    //         // block.name = "Block";
    //         block.transform.DOScale(block.transform.localScale * 1.2f, 0.1f).SetEase(Ease.OutBounce).OnComplete(() =>
    //         {
    //             block.transform.DOScale(Vector3.zero, 0.2f).OnComplete(() =>
    //             {
    //                 Renderer renderer = block.GetComponent<Renderer>();
    //                 renderer.sharedMaterial = opaqueMaterial;
    //                 // material.Clear();
    //                 // renderer.SetPropertyBlock(material);
    //                 DeleteBlock(block);
    //                 // ObjectPoolManager.ReturnObjectToPool(block);
    //                 // block.transform.SetParent(PoolBlock.transform);
    //             });
    //         });
    //         grid[x, y] = null;
    //         StartCoroutine(CheckGrid(timer * 0.1f + 0.3f));
    //     }
    //     else
    //     {
    //         if (x > 0) SetRaibowBlock(block, grid[x - 1, y].name);
    //         else
    //         {
    //             if (y < 8 && grid[x, y + 1] != null) SetRaibowBlock(block, grid[x, y + 1].name);
    //             else if (y > 0 && grid[x, y - 1] != null) SetRaibowBlock(block, grid[x, y - 1].name);
    //             else if (x < 15 && grid[x + 1, y] != null) SetRaibowBlock(block, grid[x, y - 1].name);
    //             else SetRaibowBlock(block, Random.Range(RandomRange[0], RandomRange[1] + 1).ToString());
    //         }

    //         int time = MoveBlocksInTheSpecifiedDirection();
    //     }
    // }

    // int CountUsingLINQ(List<GameObject> list)
    // {
    //     var groupedObjects = list.GroupBy(obj => obj.name);
    //     int timer = 0;
    //     foreach (var group in groupedObjects)
    //     {
    //         if(group.Count() > 1)
    //         {
    //             int count = 0;
    //             if(list.Count == group.Count()) count = DeleteListBlock(list);
    //             else
    //             {
    //                 List<GameObject> listBlock = list.FindAll(item => item.name == group.Key);
    //                 count = DeleteListBlock(listBlock);
    //             }
    //             timer = count > timer ? count : timer;
    //         }
    //     }
    //     return timer;
    // }

    // void SetRaibowBlock(GameObject block, string blockName)
    // {
    //     block.name = blockName;
    //     Renderer renderer = block.GetComponent<Renderer>();
    //     material.Clear();
    //     renderer.sharedMaterial = opaqueMaterial;

    //     textureResources.SetTexture(material, $"{CurrentTheme}.{int.Parse(blockName)}");
    //     renderer.SetPropertyBlock(material);
    // }
#endregion

#region Particle
    // public GameObject ParticlePrefab;

    IEnumerator SpawnParticle(Vector3 target)
    {
        // GameObject particle = ObjectPoolManager.SpawnObject(ParticlePrefab, Vector3.zero, Quaternion.identity);
        // particle.name = ParticlePrefab.name;
        // Transform particleTranform = particle.transform;
        // particleTranform.position = target;
        // particleTranform.SetParent(ListStar.transform);
        // particleTranform.localScale = Vector3.one;
        // particle.GetComponent<ParticleSystem>().Play();

        yield return new WaitForSeconds(2);
        // ObjectPoolManager.ReturnObjectToPool(particle);
    }

    void PlayStarAnimation (Vector3 pos, float val)
    {
        // Transform star = ObjectPoolManager.SpawnObject(StarPrefab, pos , Quaternion.identity).transform;
        // Vector3 target = gameManager.StarFrame.transform.position;
        // float distanceZ = blockSize * 0.7f;
        // star.name = "Star";
        // star.SetParent(ListStar.transform);

        // star.position = new Vector3(pos.x, pos.y, pos.z - distanceZ);
        // star.localScale = Vector3.zero;
        // totalStar += 1;
        // star.DOScale(1, 0.2f).SetEase(Ease.OutBounce).SetDelay(val);
        // star.DOMove(new Vector3(target.x- distanceZ, target.y + distanceZ, star.position.z), 0.7f)
        //     .SetDelay(val + 0.3f).SetEase(Ease.InBack).OnComplete(() =>
        //     {
        //         // audioManager.PlaySFX("Star");
        //         StartCoroutine(SpawnParticle(target));
        //         ObjectPoolManager.ReturnObjectToPool(star.gameObject);
        //         gameManager.AddStarPrice();
        //     });
    }
#endregion

#region Block Falling
    // public Canvas canvas;

    int DropHorizontalRowAnimated(int row)
    {
        int timer = 0;
        // int countItem = 0;
        for (int x = 0; x < grid.GetLength(1); x++)
        {
            GameObject cube = grid[row, x];
            if (cube)
            {
                StartCoroutine(DropCube(cube, x));
                timer = x + 1;
                grid[row, x] = null;
            }
        }
        return timer;
    }

    int DropDeleteVerticalRowAnimated(int row)
    {
        int timer = 0;
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            GameObject cube = grid[x, row];
            if (cube)
            {
                StartCoroutine(DropCube(cube, x));
                grid[x, row] = null;
                timer = x + 1;
            }
        }
        return timer;
    }

    IEnumerator DropCube(GameObject cube, int x)
    {
        yield return new WaitForSeconds(0.05f * x);
        // audioManager.PlaySFX("EatBlock");
        gameManager.CheckScore(cube.name, 1);
        RectTransform rectBody = canvas.GetComponent<RectTransform>();
        float uiHeightInUnits = rectBody.rect.y * rectBody.transform.localScale.y;
        uiHeightInUnits = cube.transform.position.y - uiHeightInUnits * 1.1f;

        Vector3 startPos = cube.transform.position;

        // Điểm mid: bay lên + lệch nhẹ x,z
        Vector3 pathPos = startPos + new Vector3(
            Random.Range(-2f, 2f),
            Random.Range(15f, 20f),
            -blockSize * 2f
        );

        // Điểm cuối: rơi xuống + lệch mạnh hơn
        Vector3 finalPos = startPos + new Vector3(
            pathPos.x * 1.2f,
            -uiHeightInUnits,
            -Random.Range(blockSize * 2.5f, blockSize * 3f)
        );

        Quaternion startRot = cube.transform.rotation;
        Quaternion targetRot = Quaternion.Euler(
            Random.Range(-180f, 180f),
            Random.Range(-180f, 180f),
            Random.Range(-180f, 180f)
        );

        float duration = Random.Range(1.5f, 2f);

        float elapsed = 0f;
        int count = 0;
        int time = 0;
        int total = 1;
        float peakTime = GetPeakTime(startPos, pathPos, finalPos);
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            if (elapsed > peakTime)
            {
                if (count < total) { count++; continue; }
                else if (count == total)
                {
                    count = 0;
                    time++;
                    if (time == 5)
                    {
                        time = 0;
                        total++;
                    }
                }
            }
            // total = (total + 1) >= 5 ? 5 : (total + 1);
            t = 1 - Mathf.Pow(1 - t, 3);

            cube.transform.position = QuadraticLerp(startPos, pathPos, finalPos, t);
            cube.transform.rotation = Quaternion.Slerp(startRot, targetRot, t);
            yield return null;
        }

        cube.transform.position = finalPos;
        cube.transform.rotation = Quaternion.identity;

        DeleteBlock(cube);
    }

    // Tìm thời điểm khi block đạt độ cao lớn nhất
    private float GetPeakTime(Vector3 start, Vector3 peak, Vector3 end)
    {
        float maxY = -1000;
        float bestT = 0;
        for (float testT = 0f; testT <= 1f; testT += Time.deltaTime)
        {
            Vector3 pos = QuadraticLerp(start, peak, end, testT);
            if (pos.y > maxY)
            {
                maxY = pos.y;
                bestT = testT;
            }
        }
        return bestT;
    }
    
    private Vector3 QuadraticLerp(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        Vector3 ab = Vector3.Lerp(a, b, t);
        Vector3 bc = Vector3.Lerp(b, c, t);
        return Vector3.Lerp(ab, bc, t);
    }
    #endregion


    void ResetListBlock()
    {
        int gridHeight = grid.GetLength(0);
        int gridWidth = grid.GetLength(1);
        for (int i = 0; i < gridHeight; i++)
        {
            for (int j = 0; j < gridWidth; j++)
            {
                GameObject block = grid[i, j];
                if (block == null) continue;
                DeleteBlock(block);
                grid[i, j] = null;
            }
        }
        blockTextureMap.Clear();

        int playerBlockChildCount = PlayerBlock.transform.childCount;
        for (int i = 0; i < 1;)
        {
            playerBlockChildCount--;
            if (playerBlockChildCount < 0) break;
            Transform block = PlayerBlock.transform.GetChild(0);
            DeleteBlock(block.gameObject);
        }
    }


    bool isDoneCoinAnimation = true;

    IEnumerator PlayWinGameAnimation()
    {
        yield return new WaitUntil(() => isDoneCoinAnimation);
        gameManager.WinGame();
    }

    IEnumerator DoneCoinAnimation(float time)
    {
        isDoneCoinAnimation = false;
        yield return new WaitForSeconds(time);
        isDoneCoinAnimation = true;
    }


    public void GameOver()
    {
        PlayerBlock.GetComponent<Movement>().StopAllAction();
        Invoke("ResetListBlock", 0.1f);
    }

    void LoseGame ()
    {
        gameManager.LoseGame();
    }
}