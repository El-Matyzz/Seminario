using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpAttackWarrior :  Ipower {

    RaycastHit hit;
    Vector3 _mousePosition;
    Vector3 _dir;
    Model _player;
	Transform _playerTransform;
    CamController _mainCamera;
	float _currentTime;
	float _jumpTime=1f;
	bool _jumpActivator;
    bool _aux;
    bool _aux2;
    Rigidbody _rb;
    float _force=7;
    float _damage=5;
    float _radius=5;

	public void Ipower()
	{
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100)) _mousePosition = hit.point;
            _mousePosition.y = _playerTransform.position.y;
            _aux = true;
            _mainCamera.sensitivityY = 1;
            _player.AnimSaltoyGolpe2();
        }
        if (_aux==true)
        {            
            _currentTime += Time.deltaTime;
            if (_jumpActivator == true)
            {
                _playerTransform.position = Vector3.Lerp(_playerTransform.position, _mousePosition, (_currentTime / _jumpTime));
                _mainCamera.cameraActivate = true;
                if (_currentTime < _jumpTime / 2) _playerTransform.position += new Vector3(0, 1, 0) * 40 * Time.deltaTime;
                if (_currentTime > _jumpTime / 2 && _currentTime < _jumpTime) _playerTransform.position += new Vector3(0, -1, 0) * 60 * Time.deltaTime;
                if (_currentTime >= _jumpTime)
                {
                    _currentTime = _jumpTime;
                    _jumpActivator = false;
                }
            }
        }

        if (_aux2==true)
        {
            _dir = _mainCamera.transform.forward;
            _dir.y = 0;
            _playerTransform.forward =Vector3.Lerp(_playerTransform.forward,_dir,0.2f);
        }



    }

    public void Ipower2()
    {
        _mainCamera.cameraActivate = false;
        Collider[] col = Physics.OverlapSphere(_playerTransform.position, _radius);
        foreach (var item in col)
        {
            if (item.GetComponent<EnemyClass>())
            {
                _rb = item.GetComponent<Rigidbody>();
                _rb.AddExplosionForce(_force, _playerTransform.position, _radius, 2, ForceMode.Impulse);
                item.GetComponent<EnemyClass>().GetDamage(_damage);
            }
        }
    }

    public JumpAttackWarrior(Vector3 mousePosition, Transform t, Model player)
	{
		_currentTime = 0;
        _player = player;
        _mainCamera = _player.mainCamera.GetComponent<CamController>();
        _mainCamera.sensitivityY = 0;
		_playerTransform =t;
        _aux2 = true;
        _jumpActivator = true;
        _player.jumpAttackWarriorState = true;
        _player.AnimSaltoyGolpe1();
        

    }
}
