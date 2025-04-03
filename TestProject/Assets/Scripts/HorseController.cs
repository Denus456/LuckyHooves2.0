using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HorseController : MonoBehaviour
{
    public int HorseNumber { get; set; }

    [SerializeField] private float _horseSpeed;
    [SerializeField] private Material[] _skinMaterials;
    [SerializeField] private GameObject _selected;

    private Animator _animator;
    private Rigidbody _rb;

    private float _baseSpeed;

    private (float, float) _firstSpeed;
    private (float, float) _secondSpeed;
    private (float, float) _thirdSpeed;

    public void StartRuning()
    {
        _animator.SetTrigger("Galope");
        GetComponent<AudioSource>().Play();
        _horseSpeed = Random.Range(_baseSpeed - 3, _baseSpeed + 3);
        StartCoroutine(ChangeHorseSpeed());
    }

    public void ActivateSelected() => _selected.SetActive(true);

    private void Start()
    {
        _selected.GetComponent<TextMeshPro>().text = HorseNumber.ToString();
        var materials = GetComponentInChildren<SkinnedMeshRenderer>().materials;
        materials[2] = _skinMaterials[Random.Range(0, _skinMaterials.Length)];
        GetComponentInChildren<SkinnedMeshRenderer>().materials = materials;

        _baseSpeed = _horseSpeed;
        _firstSpeed = CalculateSpeed();
        _secondSpeed = CalculateSpeed();
        _thirdSpeed = CalculateSpeed();

        _rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        
        _horseSpeed = 0;

    }

    private void FixedUpdate()
    {
        _rb.MovePosition(transform.position + Vector3.right * _horseSpeed * Time.fixedDeltaTime);
    }

    private  IEnumerator ChangeHorseSpeed()
    {
        float randTime = Random.Range(5f, 7f);
        yield return new WaitForSeconds(randTime);
        (_horseSpeed, _animator.speed) = _firstSpeed;
        
        randTime = Random.Range(5f, 7f);
        yield return new WaitForSeconds(randTime);
        (_horseSpeed, _animator.speed) = _secondSpeed;

        randTime = Random.Range(4f, 8f);
        yield return new WaitForSeconds(randTime);
        (_horseSpeed, _animator.speed) = _thirdSpeed;
    }

    private (float, float) CalculateSpeed()
    {
        float speed = Random.Range(_baseSpeed - Random.Range(1, 5), _baseSpeed + Random.Range(1, 5));
        float animSpeed = 1 + CalculateSpeedDiffPercent(speed);
        return (speed, animSpeed);
    }

    private float CalculateSpeedDiffPercent(float speed)
    {
        return (_baseSpeed - speed) / _baseSpeed;
    }
}
