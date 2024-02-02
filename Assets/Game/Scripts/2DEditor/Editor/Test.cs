using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.XR;
using UnityEngine.SceneManagement;

public class Test : EditorWindow
{
    [SerializeField] private static Transform _root;
    
    [MenuItem("测试/TwoDimensionalEditor")]
    private static void OpenWindow()
    {
        EditorWindow.CreateWindow<Test>();
    }

    private static GameObject _prefab;
    private static List<GameObject> _prefabList = new List<GameObject>();
    
    private static bool _isPainting = false;
    private static GameObject _selectGo;
    private static List<GameObject> _allBuildGoList = new List<GameObject>();
    
    private const int _unitCellLength = 1;
    private const int _coloumnNum = 2;
    private static bool buttonPressed = false;
    
    private const string SCENE_NAME = "EditorScene";
    
    private void OnGUI()
    {
        if (SceneManager.GetActiveScene().name != SCENE_NAME)
        {
            GUILayout.BeginVertical();
            {
                GUILayout.Label("请运行_Talk场景");
            }
            GUILayout.EndVertical();
            return;
        }
      
        GUILayout.BeginVertical();
        {
            List<Texture2D> _texs = new List<Texture2D>();
            foreach (var val in _prefabList)
            {
                Debug.Log(val);
                _texs.Add(AssetPreview.GetAssetPreview(val));
            }
            
            GUILayout.BeginHorizontal();
            {
                // foreach (var val in _texs)
                {
                    GUIStyle buttonStyle = new GUIStyle(GUI.skin.box);
                    if (GUILayout.Button(_texs[0], buttonStyle))
                    {
                        buttonPressed = true;
                        _selectGo = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefab/MyCube.prefab");
                    }
                    if (GUILayout.Button(_texs[1], buttonStyle))
                    {
                        buttonPressed = true;
                        _selectGo = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefab/MySphere.prefab");
                    }
                }
            }
            GUILayout.EndHorizontal();
                

            GUILayout.BeginHorizontal();
            {
                _isPainting = GUILayout.Toggle(_isPainting, "绘制", GUILayout.Width(60));
                // GUILayout.Toggle(_isPainting, "绘制", GUILayout.Width(60));
                if (GUILayout.Button("清空地图"))
                {
                    Debug.Log("清空");
                    ClearCell();
                }
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();
    }

    private void OnEnable()
    {
        _root = GameObject.Find("Root").transform;
        Debug.Log(_root.transform.position);
        SceneView.duringSceneGui += TestV;
        _prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefab/MyCube.prefab");
        _prefabList.Add(_prefab);
        _prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefab/MySphere.prefab");
        _prefabList.Add(_prefab);
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= TestV;
    }
    
    //主动绘制网格
    [DrawGizmo(GizmoType.NonSelected)]
    private static void OnDrawGizmos(Transform nRoot, GizmoType gizmoType)
    {
        var nTmpPos = _root.transform.position;
        for (int i = -_coloumnNum; i <= _coloumnNum; i++)
        {
            Handles.color = Color.grey;
            float nCellOffset = _unitCellLength / 2f;
            Handles.DrawLine(new Vector3(nTmpPos.x + _coloumnNum * _unitCellLength - nCellOffset, nTmpPos.y, nTmpPos.z + i * _unitCellLength - nCellOffset),
                new Vector3(nTmpPos.x - _coloumnNum * _unitCellLength - nCellOffset, nTmpPos.y, nTmpPos.z + i * _unitCellLength - nCellOffset));
            Handles.DrawLine(new Vector3(nTmpPos.x + i * _unitCellLength - nCellOffset, nTmpPos.y, nTmpPos.z + _coloumnNum * _unitCellLength - nCellOffset), 
                new Vector3(nTmpPos.x + i * _unitCellLength - nCellOffset, nTmpPos.y, nTmpPos.z - _coloumnNum * _unitCellLength - nCellOffset));
        }
    }
    
    public static void TestV(SceneView nSceneView)
    {
        if (!_isPainting)
            return;
        
        Event currentEvent = Event.current;

        // if (currentEvent.type == EventType.MouseMove)
        {
            // 获取鼠标在Scene场景中的位置
            Vector2 mousePositionSceneView = currentEvent.mousePosition;
            Camera sceneCamera = nSceneView.camera;
            HandleUtility.PushCamera(sceneCamera);
            //将鼠标位置转化成Ray对象
            Vector3Int nOutputPoint;
            if (ScenePointToWorldPoint(mousePositionSceneView, out nOutputPoint))
            {
                //绘制选中框
                Vector3 p1 = new Vector3(nOutputPoint.x - _unitCellLength / 2f, 0, nOutputPoint.z - _unitCellLength / 2f);
                Vector3 p2 = new Vector3(nOutputPoint.x - _unitCellLength / 2f, 0, nOutputPoint.z + _unitCellLength / 2f);
                Vector3 p3 = new Vector3(nOutputPoint.x + _unitCellLength / 2f, 0, nOutputPoint.z + _unitCellLength / 2f);
                Vector3 p4 = new Vector3(nOutputPoint.x + _unitCellLength / 2f, 0, nOutputPoint.z - _unitCellLength / 2f);

                Color nColor = Handles.color;
                int thickness = 2;
                Handles.color = Color.green;
                Handles.DrawLine(p1, p2, thickness);
                Handles.DrawLine(p2, p3, thickness);
                Handles.DrawLine(p3, p4, thickness);
                Handles.DrawLine(p4, p1, thickness);

                if (currentEvent.button == 0 && currentEvent.type == EventType.MouseDown)
                {
                    var aimPos = new Vector3(p1.x + _unitCellLength / 2f, 0, p1.z + _unitCellLength / 2f);
                    BuildCell(_selectGo, aimPos);
                }
                
                SceneView.RepaintAll();
                Handles.color = nColor;
            }
        }
    }

    private Dictionary<Vector2D, GameObject> _allCells = new Dictionary<Vector2D, GameObject>();
    
    private static void BuildCell(GameObject nPrefab, Vector3 nAimPos)
    {
        if (nPrefab != null)
        {
            var nGo = Instantiate(_selectGo,nAimPos, new Quaternion(0, 0, 0, 0), _root);
            _allBuildGoList.Add(nGo);
        }
    }

    private static void ClearCell()
    {
        if (_allBuildGoList == null || _allBuildGoList.Count <= 0)
            return;
        foreach (var nGo in _allBuildGoList)
        {
            GameObject.DestroyImmediate(nGo);
        }
        _allBuildGoList.Clear();
    }
    
    private static bool ScenePointToWorldPoint(Vector2 nScenePoint, out Vector3Int nOutPutPoint)
    {
        var ray = HandleUtility.GUIPointToWorldRay(nScenePoint);
        if (ray.direction.y <= 0)
        {
            float t = -ray.origin.y / ray.direction.y;
            var tmp = ray.origin + t * ray.direction;
            nOutPutPoint = new Vector3Int(Mathf.RoundToInt(tmp.x), Mathf.RoundToInt(tmp.y), Mathf.RoundToInt(tmp.z));
            // Debug.Log($"nOutPutPoint = {nOutPutPoint}");
            return true;
        }
        else
        {
            nOutPutPoint = new Vector3Int();
            return false;
        }
    }
}