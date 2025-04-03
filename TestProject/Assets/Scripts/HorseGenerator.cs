using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorseGenerator : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Generation Settings")]


    [SerializeField] private CameraFollow _mainCamera;

    [SerializeField] private GameObject _horsePrefab;
    [SerializeField] private Transform _horseBase;
    [SerializeField] private Transform _track;

    [SerializeField] private GameObject _fenceGeneratorPrefab;

    private List<HorseController> _horses = new List<HorseController>();
    private List<GameObject> _fences = new List<GameObject>();

    private float _trackWidth;
    private float _bottomLimit;
    private float _z_offset;

    private float _z_limit = 20f;

    private float _x_fence_offset = 50f;

    public List<HorseController> GenerateHorsesWithFences(int horseLinesCount, int selectedHorseNumber)
    {
        _trackWidth = _track.localScale.z * 10f - _z_limit;
        _bottomLimit = _track.position.z - _trackWidth / 2;
        _z_offset = _trackWidth / horseLinesCount;

        GameObject fisrtFenceGenerator = Instantiate(_fenceGeneratorPrefab, _horseBase);
        fisrtFenceGenerator.transform.parent = null;
        fisrtFenceGenerator.transform.position = new Vector3(_horseBase.position.x - _x_fence_offset, _horseBase.position.y, _bottomLimit);

        for (int i = 0; i < horseLinesCount; i++)
        {
            GameObject horse = Instantiate(_horsePrefab, _horseBase);
            horse.transform.parent = null;
            horse.transform.position = new Vector3(_horseBase.position.x, _horseBase.position.y, _bottomLimit + _z_offset / 2 + i * _z_offset);
            horse.GetComponent<HorseController>().HorseNumber = i + 1;

            _horses.Add(horse.GetComponent<HorseController>());

            GameObject fenceGenerator = Instantiate(_fenceGeneratorPrefab, _horseBase);
            fenceGenerator.transform.parent = null;
            fenceGenerator.transform.position = new Vector3(_horseBase.position.x - _x_fence_offset, _horseBase.position.y, _bottomLimit + (i + 1) * _z_offset);
            _fences.Add(fenceGenerator);

            if (i + 1 == selectedHorseNumber)
            {
                _mainCamera.SetTarget(horse.transform);
                horse.GetComponent<HorseController>().ActivateSelected();
            }
        }

        return _horses;
    }

    public void Restart()
    {
        _horses.ForEach(horse => { Destroy(horse.gameObject); });
        _fences.ForEach(fence => { Destroy(fence.gameObject); });

        _horses.Clear();
        _fences.Clear();
    }
}
