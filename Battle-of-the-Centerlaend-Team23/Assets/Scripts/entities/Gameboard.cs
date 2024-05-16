using System;
using System.Collections.Generic;
using communication;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using TMPro;
using Random = System.Random;


namespace entities
{
    /// <summary>
    /// This class represents the gameboard in the GameScene. It creates and displays the board in dependence of the BoardConfig that was sent by the server.
    /// </summary>
    public class Gameboard : MonoBehaviour
    {
        //hier werden die Sprites aus Unity übergeben 
        /// <summary>
        /// Takes the sprites that are set in Unity and uses them to visualise the different type of fields.
        /// </summary>
        [Header("Tiles")] [SerializeField] private TileBase hole;
        [SerializeField] private TileBase[] grass;
        [SerializeField] private TileBase lembas;
        [SerializeField] private TileBase river;
        [SerializeField] private TileBase checkpoint;
        [SerializeField] private TileBase eagle;
        [SerializeField] private TileBase start;
        [SerializeField] private TileBase wall;

        [SerializeField] private TileBase eye_north;
        [SerializeField] private TileBase eye_east;
        [SerializeField] private TileBase eye_south;
        [SerializeField] private TileBase eye_west;



        [Header("Setup Camera")]
        //camera setup variables
        public Camera mainCamera;

        private float padding = (float)0.5;

        [FormerlySerializedAs("tilemap")] [Header("Assign Tilemaps")] [SerializeField]
        private Tilemap mainTilemap;

        [SerializeField] private Tilemap wallNumberTilemap;

        [Header("Font")]
        public TMP_FontAsset _font;

        public int HEIGHT;
        private int WIDHT;
        
        private Random _random;

        void Start()
        {
            _random = new Random();
        }
    
        /// <summary>
        /// Updates the gameboard on an incoming BoardState. This only affects the LembasFields and their amount.
        /// </summary>
        /// <param name="state"></param>
        public void onBoardState(BoardState state)
        {
            int i = 0;
            foreach (var lembasField in state.lembasFields)
            {
                var pos = new Vector3Int(lembasField.position[0], HEIGHT - 1 - lembasField.position[1]);
                //lembasAmounts[new Vector3Int(lembasField.position[0], lembasField.position[1])] = lembasField.amount;
                CreateTextObject(pos, lembasField.amount.ToString());
            }
        }

/// <summary>
        /// Initializes the gameboard based on the BoardConfig. Creates a grass field according to the width and height and loops over every entry of the BoardConfig
        /// to set the correct type of tile at a position on the board.
        /// </summary>
        /// <param name="config"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void initializeTilemap(BoardConfig config)
        {
            HEIGHT = config.height;
            WIDHT = config.width;



            //initializes the size of the tilemap with all grass tiles
            for (int i = 0; i < HEIGHT; i++)
            {
                for (int j = 0; j < WIDHT; j++)
                {
                    var randomInt = _random.Next(grass.Length);
                    mainTilemap.SetTile(new Vector3Int(j, i), grass[randomInt]);
                }
            }

            //set start fields
            foreach (var startField in config.startFields)
            {
                mainTilemap.SetTile(new Vector3Int(startField.position[0], HEIGHT - 1 - startField.position[1]), start);

                int angle = 0;
                switch (startField.direction)
                {
                    case Direction.NORTH:
                        break;
                    case Direction.EAST:
                        angle = 270;
                        break;
                    case Direction.SOUTH:
                        angle = 180;
                        break;
                    case Direction.WEST:
                        angle = 90;
                        break;
                }

                //tile rotation
                Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, angle), Vector3.one);
                mainTilemap.SetTransformMatrix(
                    new Vector3Int(startField.position[0], HEIGHT - 1 - startField.position[1]), matrix);
            }

            //set hole
            for (int i = 0; i < config.holes.GetLength(0); i++)
            {
                mainTilemap.SetTile(new Vector3Int(config.holes[i, 0], HEIGHT - 1 - config.holes[i, 1]), hole);
            }

            //set checkpoints
            int checkPointCount = 0; //sets checkpoint count
            for (int i = 0; i < config.checkPoints.GetLength(0); i++)
            {
                var pos = new Vector3Int(config.checkPoints[i, 0], HEIGHT - 1 - config.checkPoints[i, 1]);

                mainTilemap.SetTile(pos, checkpoint);

                checkPointCount += 1;
                //create TextFields as Parents to display number of checkpoint
                CreateTextObject(pos, checkPointCount.ToString());

            }

            // set river field 
            foreach (var riverField in config.riverFields)
            {
                mainTilemap.SetTile(new Vector3Int(riverField.position[0], HEIGHT - 1 - riverField.position[1]), river);
                //looks at the direction and rotates the tile accordingly
                int angle = 0;
                switch (riverField.direction)
                {
                    case Direction.NORTH:
                        break;
                    case Direction.EAST:
                        angle = 270;
                        break;
                    case Direction.SOUTH:
                        angle = 180;
                        break;
                    case Direction.WEST:
                        angle = 90;
                        break;
                }

                //tile rotation
                Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, angle), Vector3.one);
                mainTilemap.SetTransformMatrix(
                    new Vector3Int(riverField.position[0], HEIGHT - 1 - riverField.position[1]), matrix);
            }

            //set LembasFields
            foreach (var lembasField in config.lembasFields)
            {
                //todo: set the amount to a text (maybe another tilemap) to the corresponding tile
                var pos = new Vector3Int(lembasField.position[0], HEIGHT - 1 - lembasField.position[1]);
                mainTilemap.SetTile(pos, lembas);

                //create TextFields as Parents to display amount of lembas
                CreateTextObject(pos, lembasField.amount.ToString());

            }

            //set eagleFields
            if (config.eagleFields is not null)
            {
                for (int i = 0; i < config.eagleFields.GetLength(0); i++)
                {
                    mainTilemap.SetTile(new Vector3Int(config.eagleFields[i, 0], HEIGHT - 1 - config.eagleFields[i, 1]),
                        eagle);
                }
            }

            //set eye to specific sprite 
            var posEye = new Vector3Int(config.eye.position[0], HEIGHT - 1 - config.eye.position[1]);
            switch (config.eye.direction)
            {
                case Direction.NORTH:
                    mainTilemap.SetTile(posEye, eye_north);
                    break;
                case Direction.EAST:
                    mainTilemap.SetTile(posEye, eye_east);
                    break;
                case Direction.SOUTH:
                    mainTilemap.SetTile(posEye, eye_south);
                    break;
                case Direction.WEST:
                    mainTilemap.SetTile(posEye, eye_west);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            //set walls - on wallNumberTilemap
            for (int i = 0; i < config.walls.GetLength(0); i++)
            {
                //first wall tuple
                int x1 = config.walls[i, 0, 0];
                int y1 = HEIGHT - 1 - config.walls[i, 0, 1];

                int x2 = config.walls[i, 1, 0];
                int y2 = HEIGHT - 1 - config.walls[i, 1, 1];

                //figure out direction of set Wall - sets opposite wall too
                if (x1 - x2 == -1)
                {
                    //East
                    setWall(x1, y1, Direction.WEST);
                    setWall(x2, y2, Direction.EAST);
                }
                else if (x1 - x2 == 1)
                {
                    //West
                    setWall(x1, y1, Direction.EAST);
                    setWall(x2, y2, Direction.WEST);
                }
                else if (y1 - y2 == -1)
                {
                    //South
                    setWall(x1, y1, Direction.NORTH);
                    setWall(x2, y2, Direction.SOUTH);
                }
                else if (y1 - y2 == 1)
                {
                    //North
                    setWall(x1, y1, Direction.SOUTH);
                    setWall(x2, y2, Direction.NORTH);
                }

            }

            //sets up the camera to fit the gameboard
            setupCamera();
        }

        /// <summary>
        /// Sets the walls on the tilemap. This uses a different tilemap than the gameboard since the walls sit on top of the fields of the board.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="direction"></param>
        private void setWall(int x, int y, Direction direction)
        {

            wallNumberTilemap.SetTile(new Vector3Int(x, y), wall);

            int angle = 0;
            switch (direction)
            {
                case Direction.NORTH:
                    break;
                case Direction.EAST:
                    angle = 90;
                    break;
                case Direction.SOUTH:
                    angle = 180;
                    break;
                case Direction.WEST:
                    angle = 270;
                    break;
            }

            //tile rotation
            Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, angle), Vector3.one);
            wallNumberTilemap.SetTransformMatrix(new Vector3Int(x, y), matrix);
        }

        /// <summary>
        /// Sets up the camera in the GameScene in dependence of the width and height.
        /// </summary>
        void setupCamera()
        {
            if (mainCamera == null)
                return;

            // calculate the new orthographic size
            float aspectRatio = mainCamera.aspect;
            float boardHeight = HEIGHT;
            float boardWidth = WIDHT;
            float verticalSize = boardHeight / 2f + padding;
            float horizontalSize = (boardWidth / 2f + padding) / aspectRatio;

            // choose the greater size to fit the whole board within view
            mainCamera.orthographicSize = Mathf.Max(verticalSize, horizontalSize);

            // Set the camera position to center the game board
            mainCamera.transform.position =
                new Vector3(boardWidth / 2f, boardHeight / 2f, mainCamera.transform.position.z);
        }
        
        //update the information on certain textObjects
        public void CreateTextObject(Vector3Int tilePosition, string newText)
        {
            GameObject searchFor = GameObject.Find("tilePosition_" + tilePosition);
            if (searchFor is not null)
            {
                searchFor.GetComponent<TextMeshPro>().text = newText;
                return;
            }
            //create new gameObject
            GameObject textObject = new GameObject("tilePosition_" + tilePosition);
            
            //change positioning
            Vector3 worldPosition = mainTilemap.CellToWorld(tilePosition) + new Vector3(0.5f, 0.5f, 0f);
            textObject.transform.position = worldPosition;
                
            //create TextMeshPro object as component of textObject and set text
            TextMeshPro tmp = textObject.AddComponent<TextMeshPro>();

            //text styling
            tmp.fontSize = 8;
            tmp.font = _font;
            tmp.fontStyle = FontStyles.Bold;
            tmp.alignment = TextAlignmentOptions.Center;
            
            //set new text
            tmp.text = newText;
            
            //bind textObject to parent and show in hirarchy
            textObject.transform.SetParent(transform);
        }
    }
}
