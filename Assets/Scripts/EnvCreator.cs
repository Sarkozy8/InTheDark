using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Unity.AI.Navigation;
using TMPro;

public class EnvCreator : MonoBehaviour
{
    [Header("Normal Prebabs")]
    public GameObject generatorPrefab;
    public GameObject pathPrefab;
    public GameObject HousePrefab;
    public GameObject wallPrefab;
    public GameObject exit20;
    public GameObject FenceDeadEnd25;
    [Header("Fences")]
    public GameObject Fence14;
    public GameObject Fence15;
    public GameObject Fence16;
    public GameObject Fence17;
    [Header("Fences Corner")]
    public GameObject FenceCorner6;
    public GameObject FenceCorner7;
    public GameObject FenceCorner8;
    public GameObject FenceCorner9;
    public GameObject FenceCorner18;
    public GameObject FenceCorner19;
    [Header("Fences DeadEnd")]
    public GameObject FenceDeadEnd10;
    public GameObject FenceDeadEnd11;
    public GameObject FenceDeadEnd12;
    public GameObject FenceDeadEnd13;
    [Header("Surface Object")]
    public NavMeshSurface navSurface;

    [Header("Default Map")]
    public Texture2D mapTexture2DDefault;

    Color[] mapPixels;
    private int[,] mapFilteredValues;
    private int[,] mapValuesCompressed;
    private string mapPreview;
    private string mapColorPreview;

    private int height = 100;
    private int width = 100;

    public int generatorCounter = 0; // Gets the total amount of generators in map


    public TextMeshProUGUI generatorCounterText;
    public GameObject _referenceOfGlobal;
    private globalReferences _globalReferences;

    public Texture2D playerMap;

    public GameObject mapDisplay;
    public float tolerance = 0.02f;
    public int downscale_tolerance = 12;

    // Start is called before the first frame update
    void Start()
    {
        _globalReferences = _referenceOfGlobal.GetComponent<globalReferences>();

        string filePath;
        playerMap = new Texture2D(2, 2);
        if (PlayerPrefs.GetInt("PLayerMap") == 1)
        {
            filePath = $"{UnityEngine.Application.persistentDataPath}/PlayerMap.png";
            // Get pixels from map
            byte[] fileData = System.IO.File.ReadAllBytes(filePath);
            playerMap.LoadImage(fileData);
            Debug.Log("Got correct map");
        }
        else
        {
            playerMap = Resources.Load<Texture2D>("MapExample");
            Debug.Log("Using Default one");
        }

        mapPixels = playerMap.GetPixels();

        Renderer renderer = mapDisplay.GetComponent<Renderer>();
        renderer.material.mainTexture = playerMap;

        // Eliminate Watermark
        for (int i = 0; i < 12500; i++)
        {
            mapPixels[i].r = 0.9647f;
            mapPixels[i].g = 0.9607f;
            mapPixels[i].b = 0.9607f;
            mapPixels[i].a = 1f;
        }

        // Iniatiate Filtered Map
        mapFilteredValues = new int[500, 500];

        // Filter Map into 0 and 1, also place them correctly (unity gives it backwards)
        for (int i = 0; i < 500; i++)
        {
            for (int p = 0; p < 500; p++)
            {
                if (compareTwoDouble(mathRoundTo3Decimal(mapPixels[p + (Math.Abs(i - 499) * 500)].r), 0.9647f, tolerance) &&
                compareTwoDouble(mathRoundTo3Decimal(mapPixels[p + (Math.Abs(i - 499) * 500)].g), 0.9607f, tolerance) &&
                compareTwoDouble(mathRoundTo3Decimal(mapPixels[p + (Math.Abs(i - 499) * 500)].b), 0.9607f, tolerance))
                {
                    mapFilteredValues[i, p] = 0;
                }
                else
                {
                    mapFilteredValues[i, p] = 1;
                }
            }
        }

        // DownscaleMap so it easier to work with
        mapValuesCompressed = DownscaleMap(mapFilteredValues);


        // Keep the largest group of 1s
        KeepLargestGroup(mapValuesCompressed);

        // Modify map (add 3s,4s, and 5s)
        mapValuesCompressed = ModifyMap(mapValuesCompressed);


        // Check if the map is big enough to be playable
        float count = 0;
        for (int i = 0; i < 100; i++)
        {
            for (int p = 0; p < 100; p++)
            {
                if (mapValuesCompressed[i, p] == 1)
                {
                    count++;
                }
            }
        }


        if ((count / 10000) > 0.05)
        {
            Debug.Log("We got the map properly! Now transfering it into Scene...");
        }
        else
        {
            Debug.Log("Map doesnt meet criteria, using premade map");
            playerMap = Resources.Load<Texture2D>("MapExample");

            mapPixels = playerMap.GetPixels();

            Renderer renderer2 = mapDisplay.GetComponent<Renderer>();
            renderer.material.mainTexture = playerMap;

            // Eliminate Watermark
            for (int i = 0; i < 12500; i++)
            {
                mapPixels[i].r = 0.9647f;
                mapPixels[i].g = 0.9607f;
                mapPixels[i].b = 0.9607f;
                mapPixels[i].a = 1f;
            }

            // Iniatiate Filtered Map
            mapFilteredValues = new int[500, 500];

            // Filter Map into 0 and 1, also place them correctly (unity gives it backwards)
            for (int i = 0; i < 500; i++)
            {
                for (int p = 0; p < 500; p++)
                {
                    if (compareTwoDouble(mathRoundTo3Decimal(mapPixels[p + (Math.Abs(i - 499) * 500)].r), 0.9647f, tolerance) &&
                    compareTwoDouble(mathRoundTo3Decimal(mapPixels[p + (Math.Abs(i - 499) * 500)].g), 0.9607f, tolerance) &&
                    compareTwoDouble(mathRoundTo3Decimal(mapPixels[p + (Math.Abs(i - 499) * 500)].b), 0.9607f, tolerance))
                    {
                        mapFilteredValues[i, p] = 0;
                    }
                    else
                    {
                        mapFilteredValues[i, p] = 1;
                    }
                }
            }

            // DownscaleMap so it easier to work with
            mapValuesCompressed = DownscaleMap(mapFilteredValues);


            // Keep the largest group of 1s
            KeepLargestGroup(mapValuesCompressed);

            // Modify map (add 3s,4s, and 5s)
            mapValuesCompressed = ModifyMap(mapValuesCompressed);

        }



        FixOrientations();
        //StartCoroutine(mapPreviewText());
        StartCoroutine(mapGeneration());

        _globalReferences.generatorDisplay.text = $"{_globalReferences.generatorActivated}/{_globalReferences.generatorCounter}";

    }


    // Update is called once per frame
    void Update()
    {

    }

    //----- Functions-Landia -----

    //--- Define directions for multiple functions
    readonly Vector2Int[] directions =
   {
        new Vector2Int(0, 1),   // Up
        new Vector2Int(1, 0),   // Right
        new Vector2Int(0, -1),  // Down
        new Vector2Int(-1, 0)   // Left
    };

    //--- Define center (house) position
    readonly Vector2Int[] centerPositions = {
        new Vector2Int(49, 49),
        new Vector2Int(49, 50),
        new Vector2Int(50, 49),
        new Vector2Int(50, 50)
    };

    IEnumerator mapGeneration()
    {
        for (int i = 0; i < 100; i++)
        {
            for (int p = 0; p < 100; p++)
            {
                //yield return new WaitForSeconds(0.0005f);
                switch (mapValuesCompressed[i, p])
                {
                    //Normal
                    case 0:
                        Instantiate(wallPrefab, new Vector3(i, 0, p), Quaternion.identity, this.transform);
                        break;
                    case 1:
                        Instantiate(pathPrefab, new Vector3(i, 0, p), Quaternion.identity, this.transform);
                        break;
                    case 3:
                        Instantiate(generatorPrefab, new Vector3(i, 0, p), Quaternion.identity, this.transform);
                        _globalReferences.generatorCounter++;
                        break;
                    case 4:
                        Instantiate(HousePrefab, new Vector3(i, 0, p), Quaternion.identity, this.transform);
                        break;
                    case 20:
                        Instantiate(exit20, new Vector3(i, 0, p), Quaternion.identity, this.transform);
                        break;
                    case 25:
                        Instantiate(FenceDeadEnd25, new Vector3(i, 0, p), Quaternion.identity, this.transform);
                        break;
                    // Count 2
                    case 6:
                        Instantiate(FenceCorner6, new Vector3(i, 0, p), Quaternion.identity, this.transform);
                        break;
                    case 7:
                        Instantiate(FenceCorner7, new Vector3(i, 0, p), Quaternion.identity, this.transform);
                        break;
                    case 8:
                        Instantiate(FenceCorner8, new Vector3(i, 0, p), Quaternion.identity, this.transform);
                        break;
                    case 9:
                        Instantiate(FenceCorner9, new Vector3(i, 0, p), Quaternion.identity, this.transform);
                        break;
                    case 18:
                        Instantiate(FenceCorner18, new Vector3(i, 0, p), Quaternion.identity, this.transform);
                        break;
                    case 19:
                        Instantiate(FenceCorner19, new Vector3(i, 0, p), Quaternion.identity, this.transform);
                        break;
                    //Count 1
                    case 14:
                        Instantiate(Fence14, new Vector3(i, 0, p), Quaternion.identity, this.transform);
                        break;
                    case 15:
                        Instantiate(Fence15, new Vector3(i, 0, p), Quaternion.identity, this.transform);
                        break;
                    case 16:
                        Instantiate(Fence16, new Vector3(i, 0, p), Quaternion.identity, this.transform);
                        break;
                    case 17:
                        Instantiate(Fence17, new Vector3(i, 0, p), Quaternion.identity, this.transform);
                        break;
                    //Count 3
                    case 10:
                        Instantiate(FenceDeadEnd10, new Vector3(i, 0, p), Quaternion.identity, this.transform);
                        break;
                    case 11:
                        Instantiate(FenceDeadEnd11, new Vector3(i, 0, p), Quaternion.identity, this.transform);
                        break;
                    case 12:
                        Instantiate(FenceDeadEnd12, new Vector3(i, 0, p), Quaternion.identity, this.transform);
                        break;
                    case 13:
                        Instantiate(FenceDeadEnd13, new Vector3(i, 0, p), Quaternion.identity, this.transform);
                        break;
                    default:
                        break;
                }
            }
        }

        navSurface.BuildNavMesh();

        Debug.Log("Map Generation is done! Congrats!");
        yield return new WaitForSeconds(0f);
    }

    void FixOrientations()
    {
        height = mapValuesCompressed.GetLength(0);
        width = mapValuesCompressed.GetLength(1);


        for (int x = 0; x < height; x++)
        {
            for (int y = 0; y < width; y++)
            {
                // Check if the current cell is 0 
                if (mapValuesCompressed[x, y] == 0)
                {
                    int count = 0;
                    int[] directs = new int[4];
                    int iteration = -1;
                    // Count adjacent cells
                    foreach (var dir in directions)
                    {

                        int nx = x + dir.x;
                        int ny = y + dir.y;
                        iteration++;
                        // Check bounds and if the neighbor is a 1
                        if (nx >= 0 && nx < height && ny >= 0 && ny < width && (mapValuesCompressed[nx, ny] == 1 || mapValuesCompressed[nx, ny] == 3))
                        {
                            directs[iteration] = 1;
                            count++;
                        }
                    }
                    // Fence Normal
                    if (count == 1)
                    {
                        //Up
                        if (directs[0] == 0 && directs[1] == 0 && directs[2] == 1 && directs[3] == 0)
                        {
                            mapValuesCompressed[x, y] = 15;
                        }
                        //Right
                        if (directs[0] == 0 && directs[1] == 0 && directs[2] == 0 && directs[3] == 1)
                        {
                            mapValuesCompressed[x, y] = 16;
                        }
                        //Down
                        if (directs[0] == 0 && directs[1] == 1 && directs[2] == 0 && directs[3] == 0)
                        {
                            mapValuesCompressed[x, y] = 17;
                        }
                        //Left
                        if (directs[0] == 1 && directs[1] == 0 && directs[2] == 0 && directs[3] == 0)
                        {
                            mapValuesCompressed[x, y] = 14;
                        }
                    }
                    //Fence Corner and sides
                    if (count == 2)
                    {
                        //Up left
                        if (directs[0] == 0 && directs[1] == 1 && directs[2] == 1 && directs[3] == 0)
                        {
                            mapValuesCompressed[x, y] = 6;
                        }
                        //up right
                        if (directs[0] == 0 && directs[1] == 0 && directs[2] == 1 && directs[3] == 1)
                        {
                            mapValuesCompressed[x, y] = 7;
                        }
                        //Down right
                        if (directs[0] == 1 && directs[1] == 0 && directs[2] == 0 && directs[3] == 1)
                        {
                            mapValuesCompressed[x, y] = 9;
                        }
                        //Down left
                        if (directs[0] == 1 && directs[1] == 1 && directs[2] == 0 && directs[3] == 0)
                        {
                            mapValuesCompressed[x, y] = 8;
                        }
                        //up down
                        if (directs[0] == 1 && directs[1] == 0 && directs[2] == 1 && directs[3] == 0)
                        {
                            mapValuesCompressed[x, y] = 19;
                        }
                        //right left
                        if (directs[0] == 0 && directs[1] == 1 && directs[2] == 0 && directs[3] == 1)
                        {
                            mapValuesCompressed[x, y] = 18;
                        }
                    }
                    // Fence DeadEnd
                    if (count == 3)
                    {
                        //Up
                        if (directs[0] == 0 && directs[1] == 1 && directs[2] == 1 && directs[3] == 1)
                        {
                            mapValuesCompressed[x, y] = 11;
                        }
                        //right
                        if (directs[0] == 1 && directs[1] == 0 && directs[2] == 1 && directs[3] == 1)
                        {
                            mapValuesCompressed[x, y] = 12;
                        }
                        //Down
                        if (directs[0] == 1 && directs[1] == 1 && directs[2] == 0 && directs[3] == 1)
                        {
                            mapValuesCompressed[x, y] = 13;
                        }
                        //left
                        if (directs[0] == 1 && directs[1] == 1 && directs[2] == 1 && directs[3] == 0)
                        {
                            mapValuesCompressed[x, y] = 10;
                        }
                    }
                }
            }
        }

        // Fix Exit having no fences
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (mapValuesCompressed[i, j] == 20) // Find the first 20
                {
                    mapValuesCompressed[49, 49] = 4;
                    mapValuesCompressed[49, 50] = 4;
                    mapValuesCompressed[50, 49] = 4;
                    mapValuesCompressed[50, 50] = 4;


                    Debug.Log($"Found 20 at ({i}, {j})");

                    // Check and modify neighbors
                    // Up
                    if (i > 0 && mapValuesCompressed[i - 1, j] >= 5)
                    {
                        mapValuesCompressed[i - 1, j] = 25;
                        Debug.Log($"Set top neighbor ({i - 1}, {j}) to 15");
                    }

                    // Right
                    if (j < mapValuesCompressed.GetLength(1) - 1 && mapValuesCompressed[i, j + 1] >= 5)
                    {
                        mapValuesCompressed[i, j + 1] = 25;
                        Debug.Log($"Set right neighbor ({i}, {j + 1}) to 16");
                    }

                    // Down
                    if (i < mapValuesCompressed.GetLength(0) - 1 && mapValuesCompressed[i + 1, j] >= 5)
                    {
                        mapValuesCompressed[i + 1, j] = 25;
                        Debug.Log($"Set bottom neighbor ({i + 1}, {j}) to 17");
                    }

                    // Left
                    if (j > 0 && mapValuesCompressed[i, j - 1] >= 5)
                    {
                        mapValuesCompressed[i, j - 1] = 25;
                        Debug.Log($"Set left neighbor ({i}, {j - 1}) to 14");
                    }

                    mapValuesCompressed[49, 50] = 5;
                    mapValuesCompressed[50, 49] = 5;
                    mapValuesCompressed[50, 50] = 5;

                }
            }
        }

    }



    //--- Compares Two doubles (to check colors)
    bool compareTwoDouble(float value1, float value2, float epsilon)
    {
        bool result = false;

        if (Math.Abs(value1 - value2) < epsilon)
        {
            result = true;
        }

        return result;
    }

    //--- Round up decimals to 3 places
    float mathRoundTo3Decimal(float value)
    {
        value = Mathf.Round(value * 1000.0f) * 0.001f;
        return value;
    }

    //--- Use for Dev, a way to visualize maps, the function itself needs to be modified to fit the array
    IEnumerator mapPreviewText()
    {
        for (int i = 0; i < 100; i++)
        {
            for (int p = 0; p < 100; p++)
            {
                mapPreview += $"{mapValuesCompressed[i, p]}, ";
            }
            mapPreview += "\n";
        }

        File.WriteAllText(Application.dataPath + "/mapPreview55.txt", mapPreview);
        yield return new WaitForSeconds(0f);
    }

    //--- Converts the 500x500 map into an 100x100 map
    int[,] DownscaleMap(int[,] largeMap)
    {
        int[,] downscaledMap = new int[100, 100];

        for (int i = 0; i < 100; i++)
        {
            for (int j = 0; j < 100; j++)
            {
                int Counts = 0;

                for (int x = i * 5; x < (i * 5) + 5; x++)
                {
                    for (int y = j * 5; y < (j * 5) + 5; y++)
                    {
                        if (largeMap[x, y] == 1)
                        {
                            Counts++;
                        }
                    }
                }

                downscaledMap[i, j] = Counts > downscale_tolerance ? 1 : 0;
            }
        }

        return downscaledMap;
    }

    //--- Creates generator (3s), house (4s), and eliminates empty space (5s)
    int[,] ModifyMap(int[,] downscaledMap)
    {
        // Define the size of the smaller grid
        int gridSize = 50;

        //Change alone 0s to 5s to optimized space
        for (int x = 0; x < mapValuesCompressed.GetLength(0); x++)
        {
            for (int y = 0; y < mapValuesCompressed.GetLength(1); y++)
            {
                // Check if the current cell is 0 and is isolated
                if (downscaledMap[x, y] == 0 && !HasAdjacentOne(x, y))
                {
                    downscaledMap[x, y] = 5; // Change isolated 0s to 5s
                }
            }
        }

        // Loop through each of the 4 grids (2x2)
        for (int gridX = 0; gridX < 2; gridX++)
        {
            for (int gridY = 0; gridY < 2; gridY++)
            {
                // Collect all positions with a "1" in the current 50x50 grid
                List<Vector2Int> positionsWithOnes = new List<Vector2Int>();

                for (int i = gridX * gridSize; i < (gridX + 1) * gridSize; i++)
                {
                    for (int j = gridY * gridSize; j < (gridY + 1) * gridSize; j++)
                    {
                        if (downscaledMap[i, j] == 1)
                        {
                            positionsWithOnes.Add(new Vector2Int(i, j));
                        }
                    }
                }

                // If we found at least one position with a "1", choose one randomly and change it to "3"
                if (positionsWithOnes.Count > 0)
                {
                    // Get a random index
                    int randomIndex = UnityEngine.Random.Range(0, positionsWithOnes.Count);
                    Vector2Int chosenPosition = positionsWithOnes[randomIndex];

                    // Change the value at the chosen position to "3"
                    downscaledMap[chosenPosition.x, chosenPosition.y] = 3;
                }
            }
        }

        downscaledMap[49, 49] = 4;
        downscaledMap[49, 50] = 4;
        downscaledMap[50, 49] = 4;
        downscaledMap[50, 50] = 4;

        EnsureCenterConnection();

        selectExit();

        downscaledMap[49, 50] = 5;
        downscaledMap[50, 49] = 5;
        downscaledMap[50, 50] = 5;

        return downscaledMap;
    }

    void selectExit()
    {
        foreach (var pos in centerPositions)
        {
            foreach (var dir in directions)
            {
                int nx = pos.x + dir.x;
                int ny = pos.y + dir.y;

                // Check if the adjacent cell is within bounds and is a 1
                if (nx >= 0 && ny >= 0 && nx < height && ny < width)
                {
                    if (mapValuesCompressed[nx, ny] == 1)
                    {
                        // Convert the first found 1 into 20
                        mapValuesCompressed[nx, ny] = 20;
                        Debug.Log($"Converted [nx: {nx}, ny: {ny}] from 1 to 20");
                        return; // Exit after converting the first 1
                    }
                }
            }
        }
    }

    //--- Finds if there is an 1 adjacent to an 0
    bool HasAdjacentOne(int x, int y)
    {
        // Use the defined directions array for checking adjacent cells
        foreach (Vector2Int dir in directions)
        {
            int nx = x + dir.x;
            int ny = y + dir.y;

            // Check bounds and if the neighbor is a 1
            if (nx >= 0 && nx < mapValuesCompressed.GetLength(0) && ny >= 0 && ny < mapValuesCompressed.GetLength(1) && mapValuesCompressed[nx, ny] == 1)
            {
                return true; // Found a neighboring 1
            }
        }

        return false; // No adjacent 1s found
    }

    //--- Finds the largest group of 1s (road), and keeps it to prevent stuff being generated and not being accesible to player
    public void KeepLargestGroup(int[,] map)
    {
        int width = map.GetLength(0);
        int height = map.GetLength(1);
        bool[,] visited = new bool[width, height];
        bool hasMultipleGroups = true;

        while (hasMultipleGroups)
        {
            hasMultipleGroups = false;
            visited = new bool[width, height];
            List<List<Vector2Int>> groups = new List<List<Vector2Int>>();

            // Step 1: Find all groups of 1s
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (map[i, j] == 1 && !visited[i, j])
                    {
                        List<Vector2Int> group = new List<Vector2Int>();
                        FloodFill(map, visited, i, j, group);
                        groups.Add(group);
                    }
                }
            }

            // Step 2: Identify the largest group
            List<Vector2Int> largestGroup = null;
            int maxSize = 0;
            foreach (var group in groups)
            {
                if (group.Count > maxSize)
                {
                    maxSize = group.Count;
                    largestGroup = group;
                }
            }

            // Step 3: Remove all groups except the largest one
            foreach (var group in groups)
            {
                if (group != largestGroup)
                {
                    hasMultipleGroups = true;
                    foreach (var pos in group)
                    {
                        map[pos.x, pos.y] = 0;
                    }
                }
            }
        }
    }

    //--- Generic Flood-fill to find all connected 1s
    void FloodFill(int[,] map, bool[,] visited, int startX, int startY, List<Vector2Int> group)
    {
        int width = map.GetLength(0);
        int height = map.GetLength(1);
        Stack<Vector2Int> stack = new Stack<Vector2Int>();

        stack.Push(new Vector2Int(startX, startY));
        visited[startX, startY] = true;

        while (stack.Count > 0)
        {
            Vector2Int cell = stack.Pop();
            group.Add(cell);

            foreach (var direction in directions)
            {
                int newX = cell.x + direction.x;
                int newY = cell.y + direction.y;

                if (newX >= 0 && newX < width && newY >= 0 && newY < height &&
                    map[newX, newY] == 1 && !visited[newX, newY])
                {
                    visited[newX, newY] = true;
                    stack.Push(new Vector2Int(newX, newY));
                }
            }
        }
    }

    //--- Simplify process to ensure that there is a connection between the house and the path
    void EnsureCenterConnection()
    {
        // Step 1: Check if any of the center 4s are already connected to a 1
        if (HasAdjacentOne(49, 49) == true || HasAdjacentOne(49, 50) == true || HasAdjacentOne(50, 49) == true || HasAdjacentOne(50, 50) == true)
        {
            Debug.Log("Center is already connected to a 1.");
            return;
        }

        // Step 2: Find the nearest 1 to the center block
        Vector2Int nearestOne = FindNearestOne();

        // Step 3: Create a path of 1s to connect the center to the nearest 1
        if (nearestOne != Vector2Int.zero)
        {
            CreatePathToNearestOne(nearestOne);
            Debug.Log("Created a path to the nearest 1.");
        }
        else
        {
            Debug.LogWarning("No 1 found in the map!");
        }
    }

    //--- Find the nearest "1" to make a path if house is not connected
    Vector2Int FindNearestOne()
    {
        // Breadth-First Search (BFS) to find the nearest 1 from the center block
        bool[,] visited = new bool[width, height];
        Queue<Vector2Int> queue = new Queue<Vector2Int>();

        foreach (var pos in centerPositions)
        {
            queue.Enqueue(pos);
            visited[pos.x, pos.y] = true;
        }

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            foreach (var dir in directions)
            {
                int nx = current.x + dir.x;
                int ny = current.y + dir.y;

                if (nx >= 0 && ny >= 0 && nx < width && ny < height && !visited[nx, ny])
                {
                    if (mapValuesCompressed[nx, ny] == 1)
                    {
                        return new Vector2Int(nx, ny);
                    }

                    if (mapValuesCompressed[nx, ny] == 0)
                    {
                        visited[nx, ny] = true;
                        queue.Enqueue(new Vector2Int(nx, ny));
                    }
                }
            }
        }

        return Vector2Int.zero; // No 1 found
    }

    //--- Creates the path to the nearest one (self explanatory xd)
    void CreatePathToNearestOne(Vector2Int target)
    {
        // Pathfinding from the center to the nearest 1 using BFS
        bool[,] visited = new bool[width, height];
        Dictionary<Vector2Int, Vector2Int> parent = new Dictionary<Vector2Int, Vector2Int>();
        Queue<Vector2Int> queue = new Queue<Vector2Int>();

        foreach (var pos in centerPositions)
        {
            queue.Enqueue(pos);
            visited[pos.x, pos.y] = true;
            parent[pos] = pos; // Set parent to itself for start positions
        }

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            if (current == target)
            {
                // Reconstruct the path from target back to the center block
                while (parent[current] != current)
                {
                    mapValuesCompressed[current.x, current.y] = 1; // Change cell to 1 to form the path
                    current = parent[current];
                }
                return;
            }

            foreach (var dir in directions)
            {
                int nx = current.x + dir.x;
                int ny = current.y + dir.y;

                if (nx >= 0 && ny >= 0 && nx < width && ny < height && !visited[nx, ny])
                {
                    visited[nx, ny] = true;
                    parent[new Vector2Int(nx, ny)] = current;
                    queue.Enqueue(new Vector2Int(nx, ny));
                }
            }
        }
    }
}

