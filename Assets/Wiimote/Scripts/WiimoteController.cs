using UnityEngine;
using System.Collections;
using WiimoteApi;

public class WiimoteController : MonoBehaviour {
	private Wiimote _wiimote;
	private bool 	_now_rumble 	= false;
	private static WiimoteController _instance;
	public	static WiimoteController Instance{
		get{
			return _instance;
		}
	}

	void Awake(){
		if (_instance == null)
			_instance = this;
		else
			Destroy (this.gameObject);
	}
	void Update () {
		/////////////////////////////////
		//*wiimoteが見つかってない場合*//
		/////////////////////////////////
		if (!WiimoteManager.HasWiimote()) {
			WiimoteManager.FindWiimotes ();
			return;
		}
		/////////////////////////////
		//*wiimoteが見つかった場合*//
		/////////////////////////////
		_wiimote = WiimoteManager.Wiimotes[0];
	}

	/// バイブレーション
	public void SetRumble(float time = 0.15f){
		StartCoroutine (_Rumble(time));
	}

	private IEnumerator _Rumble(float time){
		if(_wiimote == null){
			Debug.Log("aaaa");
			yield break;
		}
		if (_now_rumble)
			yield break;
		_now_rumble = true;
		_wiimote.RumbleOn = true;
		_wiimote.SendPlayerLED(true,true,true,true);
		yield return new WaitForSeconds (time);
		_wiimote.RumbleOn = false;
		_wiimote.SendPlayerLED(false,false,false,false);
		_now_rumble = false;
	}

	public void Reset(){
		if(WiimoteManager.HasWiimote())
			WiimoteManager.Cleanup(_wiimote);
		_wiimote = null;
	}

	/// 終了時に呼ばれるイベント関数
	void OnApplicationQuit(){
		Reset ();
	}
}
