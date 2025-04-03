using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectHorizontalGenerator : MonoBehaviour
{
    [Header("Generation Settings")]
    [SerializeField] private int _objectCount;

    [SerializeField] private GameObject _objectPrefab;
    [SerializeField] private Transform _objectBase;

    [SerializeField] private int _x_Offset = 48;

    [SerializeField] private bool _generateOnStart = false;
    public void GenerateObjects()
    {
        for (int i = 0; i < _objectCount; i++)
        {
            GameObject tribune = Instantiate(_objectPrefab, _objectBase);
            tribune.transform.position = new Vector3(_objectBase.position.x + i * _x_Offset, _objectBase.position.y, _objectBase.position.z);
        }
    }

    private void Start()
    {
        if (_generateOnStart)
        {
            GenerateObjects();
        }
    }
}
