using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using MelonLoader;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
using System.Reflection;
using System.Collections;
using System.IO;
using System.IO.Compression;
using UnityEngine.UI;
using TMPro;
using HarmonyLib;
using System.Drawing;
//For when assemblyinfo regens. You're welcome.
//using MelonLoader;
// using JohnBoltsLevelWarehouse;
//[assembly: MelonInfo(typeof(JohnBoltsLevelWarehouse.Main), "John Bolt's Level Warehouse", "v2.0.1", "Frenchy")]
// [assembly: MelonGame("Grouch", "Thunder Jumper")]
namespace JohnBoltsLevelWarehouse
{
    public class Main : MelonMod
    {
        public List<Tile> pallete = new List<Tile>();
        public List<List<int>> tiles = new List<List<int>>();
        public List<List<int>> tilesMetal = new List<List<int>>();
        public List<List<int>> tilesDeath = new List<List<int>>();
        public List<List<int>> tilesDeathMetal = new List<List<int>>();
        public Tilemap tileMap; // Grid > Tilemap GameObject
        public Tilemap InvisWalls; // Grid > Tilemap Invisible Wall
        public int width;
        public int height;
        public GameObject win;
        public string input;
        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if (sceneName == "Moonlight District")
            {
                MelonLogger.Msg("MD Loaded, running");
                MelonCoroutines.Start(WaitAndDoSomething());
            }
            if (buildIndex == 0)
            {
                CreateUI();
            }
        }
        private IEnumerator WaitAndDoSomething()
        {
            // Wait for 1 second
            yield return new WaitForSeconds(0.5f);
            SetAll(input);
            GameObject pd = GameObject.Find("Death Collider");
            PlayerDeath pds = pd.GetComponent<PlayerDeath>();
            pds.DieSilent();
        }
        private void CreateUI()
        {
            // Get the TMP_FontAsset from the Play Button text
            GameObject play = GameObject.Find("Play Button");
            TMP_Text playText = play.GetComponentInChildren<TMP_Text>();
            TMP_FontAsset tf = playText.font; // Use this TMP_FontAsset for all TMP_Text components
            GameObject canvasObject = null;

            // Create a canvas
            GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>(true); // true includes inactive objects
            foreach (GameObject obj in allObjects)
            {
                if (obj.name == "WORLD SELECT")
                {
                    canvasObject = obj;
                }
            }

            canvasObject.AddComponent<GraphicRaycaster>();

            // Create a title text object (using TextMeshProUGUI for UI text)
            GameObject titleObject = new GameObject("TitleText");
            titleObject.transform.SetParent(canvasObject.transform, false);

            TextMeshProUGUI titleText = titleObject.AddComponent<TextMeshProUGUI>(); // Use TextMeshProUGUI here
            titleText.text = "Load Custom Level";
            titleText.alignment = TextAlignmentOptions.Center;
            titleText.fontSize = 20;
            titleText.font = tf; // Set the TMP_FontAsset here
            titleText.color = new UnityEngine.Color(1, 0.812f, 0.004f, 1);

            RectTransform titleRect = titleText.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0, 0);
            titleRect.anchorMax = new Vector2(0, 0);
            titleRect.pivot = new Vector2(0, 0);
            titleRect.sizeDelta = new Vector2(300, 50);
            titleRect.anchoredPosition = new Vector2(20 - 130 + 10, 120 + 40); // Move 130 left, 40 up, and 10 right

            // Create an input field (TMP_InputField)
            GameObject inputFieldObject = new GameObject("InputField");
            RectTransform inputFieldRect = inputFieldObject.AddComponent<RectTransform>();
            inputFieldObject.transform.SetParent(canvasObject.transform, false);

            inputFieldRect.anchorMin = new Vector2(0, 0);
            inputFieldRect.anchorMax = new Vector2(0, 0);
            inputFieldRect.pivot = new Vector2(0, 0);
            inputFieldRect.sizeDelta = new Vector2(300, 50);
            inputFieldRect.anchoredPosition = new Vector2(20 - 130 + 10, 80 + 40); // Move 130 left, 40 up, and 10 right

            // Add Image for background styling
            Image inputFieldBackground = inputFieldObject.AddComponent<Image>();
            inputFieldBackground.color = new UnityEngine.Color(1, 0.812f, 0.004f, 1); // Yellow background

            // Add TMP_InputField component
            TMP_InputField inputField = inputFieldObject.AddComponent<TMP_InputField>();

            // Add a Text object as InputField's Text Component (use TextMeshProUGUI here)
            GameObject inputTextObject = new GameObject("InputText");
            inputTextObject.transform.SetParent(inputFieldObject.transform, false);

            TextMeshProUGUI inputText = inputTextObject.AddComponent<TextMeshProUGUI>(); // Use TextMeshProUGUI here
            inputText.font = tf; // Set TMP_FontAsset here
            inputText.color = UnityEngine.Color.black;
            inputText.alignment = TextAlignmentOptions.Left;
            inputText.fontSize = 20;

            RectTransform inputTextRect = inputText.GetComponent<RectTransform>();
            inputTextRect.anchorMin = new Vector2(0, 0);
            inputTextRect.anchorMax = new Vector2(1, 1);
            inputTextRect.offsetMin = new Vector2(10, 5); // Add padding inside the input field
            inputTextRect.offsetMax = new Vector2(-10, -5);
            inputField.textComponent = inputText; // Assign as the text component

            // Add Placeholder Text (use TextMeshProUGUI here)
            GameObject placeholderObject = new GameObject("Placeholder");
            placeholderObject.transform.SetParent(inputFieldObject.transform, false);

            TextMeshProUGUI placeholderText = placeholderObject.AddComponent<TextMeshProUGUI>(); // Use TextMeshProUGUI here
            placeholderText.text = "Enter level name...";
            placeholderText.font = tf; // Set TMP_FontAsset here
            placeholderText.color = UnityEngine.Color.gray;
            placeholderText.alignment = TextAlignmentOptions.Left;
            placeholderText.fontSize = 20;

            RectTransform placeholderRect = placeholderText.GetComponent<RectTransform>();
            placeholderRect.anchorMin = new Vector2(0, 0);
            placeholderRect.anchorMax = new Vector2(1, 1);
            placeholderRect.offsetMin = new Vector2(10, 5); // Add padding inside the placeholder
            placeholderRect.offsetMax = new Vector2(-10, -5);
            inputField.placeholder = placeholderText; // Assign as the placeholder

            // Add a border around the input field
            Outline inputFieldOutline = inputFieldObject.AddComponent<Outline>();
            inputFieldOutline.effectColor = UnityEngine.Color.black;
            inputFieldOutline.effectDistance = new Vector2(2, 2);

            // Create a button (TMP_Button is not needed, but we can keep it as a regular Button)
            GameObject buttonObject = new GameObject("Button");
            RectTransform buttonRect = buttonObject.AddComponent<RectTransform>();
            buttonObject.transform.SetParent(canvasObject.transform, false);

            buttonRect.anchorMin = new Vector2(0, 0);
            buttonRect.anchorMax = new Vector2(0, 0);
            buttonRect.pivot = new Vector2(0, 0);
            buttonRect.sizeDelta = new Vector2(300, 50);
            buttonRect.anchoredPosition = new Vector2(20 - 130 + 10, 20 + 40); // Move 130 left, 40 up, and 10 right

            // Add Image for background styling
            Image buttonBackground = buttonObject.AddComponent<Image>();
            buttonBackground.color = new UnityEngine.Color(1, 0.812f, 0.004f, 1); // Yellow background

            // Add Button component
            Button button = buttonObject.AddComponent<Button>();

            // Add Text component to display button label (use TextMeshProUGUI here)
            GameObject buttonTextObject = new GameObject("ButtonText");
            buttonTextObject.transform.SetParent(buttonObject.transform, false);

            TextMeshProUGUI buttonText = buttonTextObject.AddComponent<TextMeshProUGUI>(); // Use TextMeshProUGUI here
            buttonText.text = "Submit";
            buttonText.alignment = TextAlignmentOptions.Center;
            buttonText.font = tf; // Set TMP_FontAsset here
            buttonText.color = UnityEngine.Color.black;

            RectTransform textRect = buttonText.GetComponent<RectTransform>();
            textRect.sizeDelta = new Vector2(300, 50);
            textRect.anchoredPosition = Vector2.zero;

            // Add a border around the button
            Outline buttonOutline = buttonObject.AddComponent<Outline>();
            buttonOutline.effectColor = UnityEngine.Color.black;
            buttonOutline.effectDistance = new Vector2(2, 2);

            // Hook up button click event
            button.onClick.AddListener(() =>
            {
                string input = inputField.text;
                MelonLogger.Msg($"Input received: {input}");
                OnButtonClick(input); // Pass the input field text to the OnButtonClick function
            });
        }
        // Function that gets called with the text box input
        private void OnButtonClick(string userInput)
        {
            string dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Mods", "Levels", (userInput + ".tjl"));
            if (dir.EndsWith(".tjl", StringComparison.OrdinalIgnoreCase))
            {
                Debug.Log($"Extracting archive: {dir}");
                dir = ExtractArchive(dir);
                Debug.Log($"Archive extracted to: {dir}");
            }
            dir = dir.Trim('\"');
            input = dir;
            SetTilesInSet(dir);
            SetMetalTilesInSet(dir);
            SetDeathTilesInSet(dir);
            SetDeathMetalTilesInSet(dir);
            pallete = LoadPaletteFromFile(dir);
            width = int.Parse(LoadRules(dir)[1]);
            height = int.Parse(LoadRules(dir)[2]);
            StartLoadAndGetWin();
        }
        public List<string> LoadRules(string folderName)
        {
            string modsFolder = folderName;
            string filePath = Path.Combine(modsFolder, "rules.txt");
            MelonLogger.Msg($"Loading file from path: {filePath}");
            List<string> lines = new List<string>();
            // Check if the file exists
            if (File.Exists(filePath))
            {
                try
                {
                    // Read all lines from the file
                    lines = new List<string>(File.ReadAllLines(filePath));
                    MelonLogger.Msg($"File 'rules.txt' loaded successfully from folder '{folderName}'.");
                }
                catch (Exception ex)
                {
                    MelonLogger.Error($"An error occurred while loading the file: {ex.Message}");
                }
            }
            else
            {
                MelonLogger.Warning($"File 'rules.txt' not found in folder '{folderName}' inside the mods directory.");
            }

            return lines;
        }
        public void SetStartPos(Vector2 pos)
        {
            GameObject pd = GameObject.Find("Death Collider");
            PlayerDeath pds = pd.GetComponent<PlayerDeath>();
            var field = typeof(PlayerDeath).GetField("playerStartPos", BindingFlags.NonPublic | BindingFlags.Instance);
            field.SetValue(pds, pos);
        }

        public List<string> LoadTilePosFile(string path)
        {
            string filePath = Path.Combine(path, "tilepos.txt");
            MelonLogger.Msg($"Loading file from path: {filePath}");
            List<string> lines = new List<string>();

            // Check if the file exists
            if (File.Exists(filePath))
            {
                try
                {
                    // Read all lines from the file
                    lines = new List<string>(File.ReadAllLines(filePath));
                    MelonLogger.Msg($"File 'tilepos.txt' loaded successfully from folder '{path}'.");
                }
                catch (Exception ex)
                {
                    MelonLogger.Error($"An error occurred while loading the file: {ex.Message}");
                }
            }
            else
            {
                MelonLogger.Warning($"File 'tilepos.txt' not found in folder '{path}' inside the mods directory.");
            }

            return lines;
        }
        public List<string> LoadTilePosMetalFile(string folderName)
        {
            // Define the path to the specified folder inside the mods folder
            string modsFolder = folderName;
            string filePath = Path.Combine(modsFolder, "tileposGrapple.txt");
            MelonLogger.Msg($"Loading file from path: {filePath}");
            List<string> lines = new List<string>();

            // Check if the file exists
            if (File.Exists(filePath))
            {
                try
                {
                    // Read all lines from the file
                    lines = new List<string>(File.ReadAllLines(filePath));
                    MelonLogger.Msg($"File 'tileposMetal.txt' loaded successfully from folder '{folderName}'.");
                }
                catch (Exception ex)
                {
                    MelonLogger.Error($"An error occurred while loading the file: {ex.Message}");
                }
            }
            else
            {
                MelonLogger.Warning($"File 'tilepos.txt' not found in folder '{folderName}' inside the mods directory.");
            }

            return lines;
        }
        public List<string> LoadTilePosDeathMetalFile(string folderName)
        {
            // Define the path to the specified folder inside the mods folder
            string modsFolder = folderName;
            string filePath = Path.Combine(modsFolder, "tileposDeathGrapple.txt");
            MelonLogger.Msg($"Loading file from path: {filePath}");
            List<string> lines = new List<string>();

            // Check if the file exists
            if (File.Exists(filePath))
            {
                try
                {
                    // Read all lines from the file
                    lines = new List<string>(File.ReadAllLines(filePath));
                    MelonLogger.Msg($"File 'tileposDeathGrapple.txt' loaded successfully from folder '{folderName}'.");
                }
                catch (Exception ex)
                {
                    MelonLogger.Error($"An error occurred while loading the file: {ex.Message}");
                }
            }
            else
            {
                MelonLogger.Warning($"File 'tileposDeathGrapple.txt' not found in folder '{folderName}' inside the mods directory.");
            }

            return lines;
        }
        public List<string> LoadTilePosDeathFile(string folderName)
        {
            // Define the path to the specified folder inside the mods folder
            string modsFolder = folderName;
            string filePath = Path.Combine(modsFolder, "tileposDeath.txt");
            MelonLogger.Msg($"Loading file from path: {filePath}");
            List<string> lines = new List<string>();

            // Check if the file exists
            if (File.Exists(filePath))
            {
                try
                {
                    // Read all lines from the file
                    lines = new List<string>(File.ReadAllLines(filePath));
                    MelonLogger.Msg($"File 'tileposDeath.txt' loaded successfully from folder '{folderName}'.");
                }
                catch (Exception ex)
                {
                    MelonLogger.Error($"An error occurred while loading the file: {ex.Message}");
                }
            }
            else
            {
                MelonLogger.Warning($"File 'tileposDeath.txt' not found in folder '{folderName}' inside the mods directory.");
            }

            return lines;
        }
        private int GetNumericPart(string filename)
        {
            // Extract numeric part from the filename
            string numericPart = new string(filename.Where(char.IsDigit).ToArray());
            return int.TryParse(numericPart, out int result) ? result : int.MaxValue; // Non-numeric names are sorted last
        }
        public List<Tile> LoadPaletteFromFile(string folderName)
        {
            // Define the path to the Palette folder inside the specified folder in the mods directory
            string modsFolder = Path.Combine(folderName, "Palette");

            // Create a list to hold the tiles
            List<Tile> palette = new List<Tile>();
            List<string> pName = new List<string>();
            Tile emptyTile = ScriptableObject.CreateInstance<Tile>();
            emptyTile.sprite = null; // No sprite for the empty tile
            palette.Add(emptyTile);

            // Check if the folder exists
            if (Directory.Exists(modsFolder))
            {
                // Get all image files in the folder (e.g., PNG, JPG)
                string[] imageFiles = Directory.GetFiles(modsFolder, "*.*", SearchOption.TopDirectoryOnly);

                // Filter and sort files by name
                imageFiles = imageFiles
                    .Where(filePath =>
                        filePath.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                        filePath.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                        filePath.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase))
                    .OrderBy(filePath => GetNumericPart(Path.GetFileNameWithoutExtension(filePath)))
                    .ToArray();


                foreach (string filePath in imageFiles)
                {
                    try
                    {
                        MelonLogger.Msg($"Loading file: {filePath} to slot " + palette.Count);
                        byte[] fileData = File.ReadAllBytes(filePath);
                        Texture2D texture = new Texture2D(2, 2);
                        texture.LoadImage(fileData); // Ensure the texture is populated

                        MelonLogger.Msg($"Texture size: {texture.width}x{texture.height}");
                        float pixelsPerUnit = texture.width; // Use a constant or configurable value

                        // Create a Sprite from the Texture2D
                        Rect rect = new Rect(0, 0, texture.width, texture.height);
                        Sprite sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f), pixelsPerUnit);

                        // Create a Tile from the Sprite
                        Tile tile = ScriptableObject.CreateInstance<Tile>();
                        tile.sprite = sprite; 

                        // Add the tile to the palette list
                        palette.Add(tile);
                        pName.Add(filePath);
                    }
                    catch (Exception ex)
                    {
                        MelonLogger.Error($"Failed to load image file: {filePath}, Error: {ex.Message}");
                    }
                }
            }
            else
            {
                MelonLogger.Warning($"Palette folder '{modsFolder}' not found.");
            }
            MelonLogger.Msg(palette.Count);
            MelonLogger.Msg(string.Join(",", pName));
            return palette;
        }
        public void SetMetalTilesInSet(string name)
        {
            List<string> temp = LoadTilePosMetalFile(name);
            if (temp == null || temp.Count == 0)
            {
                MelonLogger.Error("Failed to load lines from the file or the file is empty.");
                return;
            }

            List<List<int>> fullTemp = new List<List<int>>();

            foreach (string item in temp)
            {
                // Split the string by commas and trim any whitespace
                List<int> splitList = new List<int>();
                foreach (string str in item.Split(','))
                {
                    if (int.TryParse(str.Trim(), out int result))
                    {
                        splitList.Add(result);
                    }
                }

                // Add the split list to the 2D list
                fullTemp.Add(splitList);
            }

            tilesMetal = fullTemp;
        }
        public void SetDeathMetalTilesInSet(string name)
        {
            List<string> temp = LoadTilePosDeathMetalFile(name);
            if (temp == null || temp.Count == 0)
            {
                MelonLogger.Error("Failed to load lines from the file or the file is empty.");
                return;
            }

            List<List<int>> fullTemp = new List<List<int>>();

            foreach (string item in temp)
            {
                // Split the string by commas and trim any whitespace
                List<int> splitList = new List<int>();
                foreach (string str in item.Split(','))
                {
                    if (int.TryParse(str.Trim(), out int result))
                    {
                        splitList.Add(result);
                    }
                }

                // Add the split list to the 2D list
                fullTemp.Add(splitList);
            }

            tilesDeathMetal = fullTemp;
        }
        public void SetDeathTilesInSet(string name)
        {
            List<string> temp = LoadTilePosDeathFile(name);
            if (temp == null || temp.Count == 0)
            {
                MelonLogger.Error("Failed to load lines from the file or the file is empty.");
                return;
            }

            List<List<int>> fullTemp = new List<List<int>>();

            foreach (string item in temp)
            {
                // Split the string by commas and trim any whitespace
                List<int> splitList = new List<int>();
                foreach (string str in item.Split(','))
                {
                    if (int.TryParse(str.Trim(), out int result))
                    {
                        splitList.Add(result);
                    }
                }

                // Add the split list to the 2D list
                fullTemp.Add(splitList);
            }

            tilesDeath = fullTemp;
        }
        public void SetTilesInSet(string name)
        {
            List<string> temp = LoadTilePosFile(name);
            if (temp == null || temp.Count == 0)
            {
                MelonLogger.Error("Failed to load lines from the file or the file is empty.");
                return;
            }

            List<List<int>> fullTemp = new List<List<int>>();

            foreach (string item in temp)
            {
                // Split the string by commas and trim any whitespace
                List<int> splitList = new List<int>();
                foreach (string str in item.Split(','))
                {
                    if (int.TryParse(str.Trim(), out int result))
                    {
                        splitList.Add(result);
                    }
                }

                // Add the split list to the 2D list
                fullTemp.Add(splitList);
            }
            MelonLogger.Msg(string.Join("; ", fullTemp.Select(innerList => 
                $"[{string.Join(", ", innerList)}]"
            )));
            tiles = fullTemp;
        }
        public void ClearAllTilesManually(string name)
        {
            MelonLogger.Msg("ClearAllTilesManually method called.");

            // Try to find the Tilemap GameObject
            GameObject tilemapGameObject = GameObject.Find(name);

            if (tilemapGameObject == null)
            {
                MelonLogger.Error("Tilemap GameObject not found.");
                return;
            }

            // Try to get the Tilemap component
            tileMap = tilemapGameObject.GetComponent<Tilemap>();

            if (tileMap == null)
            {
                MelonLogger.Error("Tilemap component not found on Tilemap GameObject.");
                return;
            }

            // Iterate over all tile positions and set them to null
            BoundsInt bounds = tileMap.cellBounds;
            for (int x = bounds.xMin; x < bounds.xMax; x++)
            {
                for (int y = bounds.yMin; y < bounds.yMax; y++)
                {
                    for (int z = bounds.zMin; z < bounds.zMax; z++)
                    {
                        tileMap.SetTile(new Vector3Int(x, y, z), null);
                    }
                }
            }

            tileMap.RefreshAllTiles();
            MelonLogger.Msg("All tiles cleared manually and Tilemap refreshed.");
        }
        private string FormatNestedList(List<List<int>> nestedList)
        {
            return string.Join("; ", nestedList.Select(innerList => 
                $"[{string.Join(", ", innerList)}]"
            ));
        }
        public void SetAll(string userInput)
        {
            MelonLogger.Msg("SetAll method called.");
            SetStartPos(new Vector2(float.Parse(LoadRules(userInput)[3]), float.Parse(LoadRules(userInput)[4])));
            // Try to find the Tilemap GameObject
            string filePath = Path.Combine(userInput, "other.txt");
            string[] lines = File.ReadAllLines(filePath);
            foreach (string line in lines)
            {
                ProcessLine(line);
            }
            GameObject tilemapGameObject = GameObject.Find("Tilemap Gameobject");
            GameObject coverT = GameObject.Find("Ground Cover");
            GameObject coverTM = GameObject.Find("Ground Cover Metal");
            if (tilemapGameObject == null)
            {
                MelonLogger.Error("Tilemap GameObject not found.");
                return;
            }
            ClearAllTilesManually("Tilemap Gameobject");
            ClearAllTilesManually("Ground Cover");
            ClearAllTilesManually("Ground Cover Metal");

            // Try to get the Tilemap component
            tileMap = tilemapGameObject.GetComponent<Tilemap>();
            Tilemap gct = coverT.GetComponent<Tilemap>();
            Tilemap gctm = coverTM.GetComponent<Tilemap>();

            if (tileMap == null)
            {
                MelonLogger.Error("Tilemap component not found on Tilemap GameObject.");
                return;
            }

            if (tiles == null || tiles.Count == 0)
            {
                MelonLogger.Error("Tiles list is empty or not initialized.");
                return;
            }

            while (tilemapGameObject.transform.childCount > 0)
            {
                GameObject.DestroyImmediate(tilemapGameObject.transform.GetChild(0).gameObject);
            }
            PrepMD();
            GroundCoverController gcc = coverT.GetComponent<GroundCoverController>();
            GroundCoverController gccm = coverTM.GetComponent<GroundCoverController>();
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (i < tiles.Count && j < tiles[i].Count)
                    {
                        if (tiles[i][j] != 0)
                        {
                            tileMap.SetTile(new Vector3Int(j - width / 2, height - i, 0), pallete[tiles[i][j]]);
                            MelonLogger.Msg("tiles i j is " + tiles[i][j]);
                            MelonLogger.Msg($"Placing tile {pallete[tiles[i][j]]} at position ({j}, {i})");
                            gct.SetTile(new Vector3Int(j- width / 2, height - i, 0), gcc.groundTile);
                            GameObject temp = new GameObject();
                            temp.transform.parent = tileMap.transform;
                            temp.name = "Rubber";
                            temp.transform.localScale = new Vector3(0.5f, 1, 0.5f);
                            temp.tag = "Rubber";
                            temp.layer = 8;
                            temp.transform.position = new Vector3Int(j- width / 2, height - i, 0);
                        }
                    }
                    else
                    {
                        MelonLogger.Error($"Index out of range: i={i}, j={j}");
                    }
                }
            }
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (i < tiles.Count && j < tiles[i].Count)
                    {
                        if (tilesMetal[i][j] != 0)
                        {
                            tileMap.SetTile(new Vector3Int(j- width / 2, height - i, 0), pallete[tilesMetal[i][j]]);
                            gctm.SetTile(new Vector3Int( j- width / 2,height -  i, 0), gccm.groundTile);
                            GameObject temp = new GameObject();
                            temp.transform.parent = tileMap.transform;
                            temp.name = "Metal";
                            temp.transform.localScale = new Vector3(0.5f, 1, 0.5f);
                            temp.tag = "Metal";
                            temp.layer = 8;
                            temp.transform.position = new Vector3Int(j- width / 2, height - i, 0);
                        }
                    }
                    else
                    {
                        MelonLogger.Error($"Index out of range: i={i}, j={j}");
                    }
                }
            }
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (i < tiles.Count && j < tiles[i].Count)
                    {
                        if (tilesDeath[i][j] != 0)
                        {
                            tileMap.SetTile(new Vector3Int( j- width / 2,height -  i, 0), pallete[tilesDeath[i][j]]);
                            GameObject temp = new GameObject();
                            temp.transform.parent = tileMap.transform;
                            temp.name = "Death Rubber";
                            //temp.transform.localScale = new Vector3(0.5f, 1, 0.5f);
                            temp.tag = "Death Rubber";
                            temp.layer = 8;
                            BoxCollider2D box = temp.AddComponent<BoxCollider2D>();
                            box.offset = new Vector2(1f, 1f);
                            temp.transform.position = new Vector3Int(j- width / 2, height - i, 0);
                        }
                    }
                    else
                    {
                        MelonLogger.Error($"Index out of range: i={i}, j={j}");
                    }
                }
            }
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (i < tiles.Count && j < tiles[i].Count)
                    {
                        if (tilesDeathMetal[i][j] != 0)
                        {
                            tileMap.SetTile(new Vector3Int(j- width / 2, height - i, 0), pallete[tilesDeathMetal[i][j]]);
                            GameObject temp = new GameObject();
                            temp.transform.parent = tileMap.transform;
                            temp.name = "Death Metal";
                            //temp.transform.localScale = new Vector3(0.5f, 1, 0.5f);
                            temp.tag = "Death";
                            temp.layer = 8;
                            BoxCollider2D box = temp.AddComponent<BoxCollider2D>();
                            box.offset = new Vector2(1f, 1f);
                            temp.transform.position = new Vector3Int(j- width / 2, height - i, 0);
                        }
                    }
                    else
                    {
                        MelonLogger.Error($"Index out of range: i={i}, j={j}");
                    }
                }
            }
            tileMap.gameObject.transform.position = new Vector2(-0.5f, -0.5f);
            /*tileMap.gameObject.GetComponent<TilemapCollider2D>().enabled = true;
            GameObject cgc = GameObject.Find("Ground Cover");
            if (cgc != null)
            {
                GroundCoverController groundCoverController = cgc.GetComponent<GroundCoverController>();
                if (groundCoverController != null)
                {
                    // Access and invoke the CoverAllGround method using reflection
                    MethodInfo coverAllGroundMethod = typeof(GroundCoverController).GetMethod("CoverAllGround", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    if (coverAllGroundMethod != null)
                    {
                        coverAllGroundMethod.Invoke(groundCoverController, null);
                        MelonLogger.Msg("CoverAllGround method invoked.");
                    }
                    else
                    {
                        MelonLogger.Warning("Method 'CoverAllGround' not found.");
                    }
                }
                else
                {
                    MelonLogger.Warning("GroundCoverController component not found.");
                }
                MelonLogger.Msg(GetTileCount(cgc.GetComponent<Tilemap>()));
            }
            else
            {
                MelonLogger.Warning("Ground cover object is null.");
            }
            GameObject cgcm = GameObject.Find("Ground Cover Metal");
            if (cgcm != null)
            {
                GroundCoverController groundCoverController2 = cgcm.GetComponent<GroundCoverController>();
                if (groundCoverController2 != null)
                {
                    // Access and invoke the CoverAllGround method using reflection
                    MethodInfo coverAllGroundMethod = typeof(GroundCoverController).GetMethod("CoverAllGround", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    if (coverAllGroundMethod != null)
                    {
                        coverAllGroundMethod.Invoke(groundCoverController2, null);
                        MelonLogger.Msg("CoverAllGround method invoked.");
                    }
                    else
                    {
                        MelonLogger.Warning("Method 'CoverAllGround' not found.");
                    }
                }
                else
                {
                    MelonLogger.Warning("GroundCoverController component not found.");
                }
                MelonLogger.Msg(GetTileCount(cgcm.GetComponent<Tilemap>()));
            }
            else
            {
                MelonLogger.Warning("Ground cover object is null.");
            }
            cgc.GetComponent<TilemapCollider2D>().enabled = false;
            cgc.GetComponent<TilemapCollider2D>().enabled = true;
            cgc.GetComponent<CompositeCollider2D>().enabled = false;
            cgc.GetComponent <CompositeCollider2D>().enabled = true;
            cgcm.GetComponent<TilemapCollider2D>().enabled = false;
            cgcm.GetComponent<TilemapCollider2D>().enabled = true;
            cgcm.GetComponent<CompositeCollider2D>().enabled = false;
            cgcm.GetComponent<CompositeCollider2D>().enabled = true;*/
            GameObject p = GameObject.Find("Player B3");
            MelonLogger.Msg("After player objkect");
            Rigidbody2D prb = p.GetComponent<Rigidbody2D>();
            MelonLogger.Msg("After player rb");
            prb.gravityScale = 2;
            MelonLogger.Msg(prb.gravityScale);
        }
        public int GetTileCount(Tilemap tilemap)
        {
            int tileCount = 0;
            if (tilemap != null)
            {
                // Get the bounds of the tilemap
                BoundsInt bounds = tilemap.cellBounds;

                // Iterate through all positions within the bounds
                foreach (var pos in bounds.allPositionsWithin)
                {
                    if (tilemap.HasTile(pos))
                    {
                        tileCount++;
                    }
                }
            }
            else
            {
                MelonLogger.Warning("Tilemap is null.");
            }
            return tileCount;
        }
        void ProcessLine(string line)
        {
            // Split the line by commas
            string[] parts = line.Split(',');

            if (parts.Length != 3)
            {
                MelonLogger.Error($"Invalid line format: {line}");
                return;
            }

            // Parse values
            string command = parts[0].Trim();
            if (!float.TryParse(parts[1].Trim(), out float param1) || !float.TryParse(parts[2].Trim(), out float param2))
            {
                MelonLogger.Error($"Invalid numeric values in line: {line}");
                return;
            }

            // Handle the command
            switch (command.ToLower())
            {
                case "win":
                    HandleWin(param1, param2);
                    break;
                case "checkpoint":
                    HandleCheckpoint(param1, param2);
                    break;
                default:
                    MelonLogger.Warning($"Unknown command: {command}");
                    break;
            }
        }
        // Example functions to handle commands
        void HandleWin(float param1, float param2)
        {
            GameObject temp = GameObject.Find("Win");
            SceneManager.MoveGameObjectToScene(temp, SceneManager.GetActiveScene());
            temp.transform.position = new Vector2(param1, param2);
            temp.SetActive(true);
        }
        

        void HandleCheckpoint(float param1, float param2)
        {
            GameObject checkPoint = GameObject.Find("Checkpoint");
            GameObject cp = GameObject.Instantiate(checkPoint);
            cp.transform.position = new Vector2(param1, param2);
            cp.SetActive(true);
        }
        public string ExtractArchive(string archivePath)
        {
            string tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDirectory);
            string tempZipPath = Path.Combine(tempDirectory, "temp.zip");
            File.Copy(archivePath, tempZipPath);
            ZipFile.ExtractToDirectory(tempZipPath, tempDirectory);
            string[] dirs = Directory.GetDirectories(tempDirectory);
            return tempDirectory;
        }
        public void CleanupTemporaryDirectory(string tempDir)
        {
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
                Debug.Log($"Temporary directory deleted: {tempDir}");
            }
        }
        public void PrepMD()
        {
            GameObject tiwd = GameObject.Find("Tilemap Invisible Wall Death");
            tiwd.SetActive(false);
            GameObject spm = GameObject.Find("Sky Particles Moon");
            spm.SetActive(false);
            GameObject b = GameObject.Find("Bolt");
            b.SetActive(false);
            GameObject b1 = GameObject.Find("Bolt (1)");
            b1.SetActive(false);
            GameObject b2 = GameObject.Find("Bolt (2)");
            b2.SetActive(false);
            GameObject b3 = GameObject.Find("Bolt (3)");
            b3.SetActive(false);
            GameObject b4 = GameObject.Find("Bolt (4)");
            b4.SetActive(false);
            GameObject psa = GameObject.Find("Prop Sign Arrow");
            psa.SetActive(false);
            GameObject cc = GameObject.Find("Cloud Cannon");
            cc.SetActive(false);
            GameObject cc1 = GameObject.Find("Cloud Cannon (1)");
            cc1.SetActive(false);
            GameObject cc2 = GameObject.Find("Cloud Cannon (2)");
            cc2.SetActive(false);
            GameObject m = GameObject.Find("Metals");
            m.SetActive(false);
            GameObject c = GameObject.Find("Checkpoint");
            c.SetActive(false);
            GameObject c1 = GameObject.Find("Checkpoint (1)");
            c1.SetActive(false);
            GameObject c2 = GameObject.Find("Checkpoint (2)");
            c2.SetActive(false);
            GameObject c3 = GameObject.Find("Checkpoint (3)");
            c3.SetActive(false);
            MelonLogger.Msg("Before player");
            GameObject p = GameObject.Find("Player B3");
            MelonLogger.Msg("After player objkect");
            Rigidbody2D prb = p.GetComponent<Rigidbody2D>();
            MelonLogger.Msg("After player rb");
            prb.gravityScale = 2; 
            var playerSwimming = p.GetComponent("PlayerSwimming");
            if (playerSwimming == null)
            {
                MelonLogger.Error("PlayerSwimming component not found on Player B3!");
                return;
            }
            FieldInfo startGravField = playerSwimming.GetType().GetField("startGrav", BindingFlags.NonPublic | BindingFlags.Instance);
            if (startGravField == null)
            {
                MelonLogger.Error("Private field 'startGrav' not found in PlayerSwimming!");
                return;
            }

            // Set the value of the private field
            startGravField.SetValue(playerSwimming, 2f);
            GameObject glider = GameObject.Find("Star Screamer");
            Shift_StarScreamer gs = glider.GetComponent<Shift_StarScreamer>();
            FieldInfo defaultGravField = gs.GetType().GetField("defaultGravity", BindingFlags.NonPublic | BindingFlags.Instance);
            defaultGravField.SetValue(gs, 2f);
            MelonLogger.Msg(prb == null);
            MelonLogger.Msg(p == null);
            MelonLogger.Msg(prb.gravityScale);
            MelonLogger.Msg("After set GScale");
        }
        public void StartLoadAndGetWin()
        {
            // Start the coroutine to load and fetch the "Win" GameObject
            MelonCoroutines.Start(LoadSceneAndGetWin());
        }

        private IEnumerator LoadSceneAndGetWin()
        {
            // Load scene asynchronously
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(1);

            // Wait until the scene is fully loaded
            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            // Try to find the "Win" GameObject
            GameObject winObject = GameObject.Find("Win");
            GameObject.DontDestroyOnLoad(winObject);

            // Optionally, load back the original scene or perform other operations
            SceneManager.LoadScene("Moonlight District");
            MelonLogger.Msg($"Win is now {(win != null ? "not null" : "null")}");
        }
    }
    [HarmonyPatch(typeof(CheatCodeManager), "Update")]
    static class patch
    {
        static bool Prefix()
        {
            return false;
        }
    }
}
