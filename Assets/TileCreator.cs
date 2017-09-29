using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

namespace Assets
{
    public class TileCreator : MonoBehaviour
    {
        private GameObject[][] _tiles;
        public GameObject Tile;
        public List<GameObject> InfectedTiles = new List<GameObject>(); 
        public List<GameObject> ObjectivesTiles = new List<GameObject>(); 
        public int GridWidth = 10;
        public int GridHeight = 10;
        public float StartX = 0;
        public float StartY = 0;
        public Text TxtResetNum;
		private int _turnsLeft;
		public const int MAX_TURNS = 3;
		public Text TxtLeft;
        public GameObject WinPopup;
        public int InfectionProgress = 2;
        public int Objectives = 0;
        public int Score = 0;
        public Text ScoreText;

        public float CameraLimitX;
        public float CameraLimitY;
    

        // Use this for initialization
        void Start ()
        {
            ResetField();
        }

        void SetUpCamera()
        {
            for (var i = 0; i <= _tiles.Length - 1; i++)
            {
                for (var j = 0; j <= _tiles[i].Length - 1; j++)
                {
                    if (_tiles[i][j].GetComponent<TileInfo>().Status == 0)
                    {
                        var t = _tiles[i][j].transform;

                        if (t.position.x > CameraLimitX)
                            CameraLimitX = t.position.x;

                        if (t.position.y > CameraLimitY)
                            CameraLimitY = t.position.y;
                    }
                }
            }
            Debug.Log("Camera Limits are : "+CameraLimitX + " - "+CameraLimitY);
        }

        void Update()
        {
            if (Input.GetKeyUp(KeyCode.Return))
            {
                EndTurn();
            }

            if (Input.GetKeyUp(KeyCode.R))
            {
                ResetField();
            }
        }

        public void ResetField()
        {
            CheckLevel();
            CreateGrid();
            CreateInfection(GetInfectionNum());
            ResetTurns();
            WinPopUpDeActive();
            UpdateScoreText();
            CreateObjectives(Objectives);
            SetUpCamera();
        }

        private void UpdateScoreText()
        {
            ScoreText.text = Score.ToString();
        }

        private void CheckLevel()
        {
            if (Application.loadedLevelName == "Quick")
            {
                InfectionProgress++;
                Objectives++;
            }
        }

        private int GetInfectionNum()
        {
            int x;

            if (TxtResetNum != null)
            {
                if (Int32.TryParse(TxtResetNum.text, out x))
                    return x;
                else
                    return 3; 
            }
            else
            {
                return InfectionProgress;
            }
                 
        }

        public void EndTurn()
        {
            SpreadInfection();
            ResetTurns();
        }

        private void SpreadInfection()
        {
            List<GameObject> NewInfections = new List<GameObject>();
            
            foreach (var infectedTile in InfectedTiles)
            {
                //check adjacents for list of possibles
                List<GameObject> PossibleInfections = new List<GameObject>();
                foreach (var adj in infectedTile.GetComponent<TileInfo>().Adjacents)
                {
                    if(adj != null)
                        if (adj.GetComponent<TileInfo>().Status == 0 || adj.GetComponent<TileInfo>().Status == 3)
                            PossibleInfections.Add(adj);
                }

                if (PossibleInfections.Count > 0)
                {
                    GameObject dead = PossibleInfections[UnityEngine.Random.Range(0, PossibleInfections.Count - 1)];
                    NewInfections.Add(dead);
                }  
            }

            if(NewInfections.Count == 0)
            {
                WinPopUpActive();

                IncreaseScore();
            }
            else
            {
                foreach (var g in NewInfections)
                {
                    InfectedTiles.Add(g);
                }

                foreach (GameObject g in InfectedTiles)
                {
                    g.GetComponent<TileInfo>().SetDead();
                }
            }            
        }

        private void IncreaseScore()
        {
            for (int i = 0; i <= _tiles.Length - 1; i++)
            {
                for (int j = 0; j <= _tiles[i].Length - 1; j++)
                {
                    if (_tiles[i][j].GetComponent<TileInfo>().Status == 0)
                        Score++;
                    if (_tiles[i][j].GetComponent<TileInfo>().Status == 3)
                        Score+= 10;
                }
            }
        }

        void CreateInfection(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                InfectedTiles.Add(_tiles[UnityEngine.Random.Range(0, GridWidth)][UnityEngine.Random.Range(0, GridHeight)]);
            }

            foreach (GameObject g in InfectedTiles)
            {
                g.GetComponent<TileInfo>().SetDead();
            }
        }

        void CreateObjectives(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                ObjectivesTiles.Add(_tiles[UnityEngine.Random.Range(0, GridWidth)][UnityEngine.Random.Range(0, GridHeight)]);
            }

            foreach (GameObject g in ObjectivesTiles)
            {
                g.GetComponent<TileInfo>().SetTarget();
            }
        }

        void CreateGrid()
        {
            InfectedTiles.Clear();
            SetUpArray();
            RemoveChildren();
            CreateTiles();
        }

        private void CreateTiles()
        {
            //set up gameobjects
            for (int i = 0; i <= GridWidth - 1; i++)
                for (int j = 0; j <= GridHeight - 1; j++)
                {
                    GameObject temp = Instantiate(Tile, GetTileLocation(i, j), Quaternion.identity) as GameObject;
                    temp.name = i + "," + j;
                    _tiles[i][j] = temp;
                    TileInfo info = temp.GetComponent<TileInfo>();
                    info.PosX = i;
                    info.PosY = j;
                    info.SetTileCreator(this);
                }

            //set up adjacencies for gameobjects
            for (int i = 0; i <= GridWidth - 1; i++)
                for (int j = 0; j <= GridHeight - 1; j++)
                {
                    SetUpAdjacents(i, j);
                }
        }

        private void SetUpAdjacents(int i, int j)
        {
            TileInfo info = _tiles[i][j].GetComponent<TileInfo>();

            if (i - 1 >= 0)
                info.Adjacents[0] = _tiles[i - 1][j];

            if (i + 1 <= _tiles.Length-1)
                info.Adjacents[1] = _tiles[i + 1][j];

            if (j - 1 >= 0)
                info.Adjacents[2] = _tiles[i][j - 1];

            if (j + 1 <= _tiles[i].Length - 1)
                info.Adjacents[3] = _tiles[i][j + 1];
        }

        private Vector3 GetTileLocation(int i, int j)
        {
            Vector3 temp = new Vector3(StartX+i,StartY+j);
            return temp;
        }

        private void RemoveChildren()
        {
            foreach (Transform t in transform)
            {
                Destroy(t.gameObject);
            }
        }

        private void SetUpArray()
        {
            _tiles = new GameObject[GridWidth][];

            for (int i = 0; i <= GridWidth - 1; i++)
            {
                _tiles[i] = new GameObject[GridHeight];
            }
        }

        public bool OnTileClick(int x, int y)
        {
            if(_turnsLeft >= 1)
            {
                _turnsLeft--;
                TxtLeft.text = _turnsLeft.ToString();
                return true;
            }
            else
            {
                return false;
            }
			
        }

        void ResetTurns()
        {
            _turnsLeft = MAX_TURNS;
            TxtLeft.text = _turnsLeft.ToString();

        }

        void WinPopUpActive()
        {
            WinPopup.SetActive(true);
        }

        void WinPopUpDeActive()
        {
            WinPopup.SetActive(false);
        }
    }
}
