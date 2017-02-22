using UnityEngine;
using System.Collections;
using WiimoteApi;

////////////////////////////////
/// Wii_Motion_Interface.cs	///
///////////////////////////////
public class Wii_Motion_Interface : MonoBehaviour {
	[SerializeField]
	private float	_rumble_time	= 0.5f;
	private Wiimote _wiimote;
	private Vector3 _accel 			= Vector3.zero;
	private bool 	_now_rumble 	= false;
	private bool	_wiimote_reset	= false;
	/////////////////////////////////////////
	/// 最新の_accelセンサ値を取得できます///
	/////////////////////////////////////////
	public Vector3 Accel{
		get{
			return _accel;
		}
	}
	void Update () {
		if (_wiimote_reset)
			return;
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
		bool init = ( _wiimote == null );
		_wiimote = WiimoteManager.Wiimotes[0];
		if(init && _wiimote != null) {
			_Initialize();
			return;
		}
		GetAccelData ();
		if (Input.anyKey) {
			SetRumble ();
		}
		this.transform.rotation = Quaternion.Euler (_accel);
	}

	//////////////
	//*初期設定*//
	//////////////
	private void _Initialize(){
		_wiimote.SendDataReportMode(InputDataType.REPORT_EXT21);
		_wiimote.RequestIdentifyWiiMotionPlus ();
		_wiimote.ActivateWiiMotionPlus ();
	}

	/// バイブレーション
	/// 引数振動時間(秒)
	public void SetRumble(){
		StartCoroutine ("_Rumble");
	}

	private IEnumerator _Rumble(){
		if (_now_rumble || _wiimote_reset) 
			yield break;
		_now_rumble = true;
		_wiimote.RumbleOn = true;
		_wiimote.SendStatusInfoRequest();
		yield return new WaitForSeconds (_rumble_time);
		_wiimote.RumbleOn = false;
		_wiimote.SendStatusInfoRequest();
		Reset ();
		_now_rumble = false;
	}

	////////////////////////////
	///*加速度データ取得処理*///
	////////////////////////////
	private Vector3 GetAccelData(){
		int ret;
		Vector3 offset = Vector3.zero;
		do
		{
			ret = _wiimote.ReadWiimoteData();
			if(ret > 0 && _wiimote.current_ext == ExtensionController.MOTIONPLUS ){
				MotionPlusData data = _wiimote.MotionPlus;
				offset = new Vector3
					(
						data.PitchSpeed,
						data.YawSpeed,
						data.RollSpeed
					) / 95f;
				_accel += offset;
			}
		} while (ret > 0);
		return offset;
	}

	public void Reset(){
		StartCoroutine ("_Reset");
	}

	private IEnumerator _Reset(){
		_wiimote_reset = true;
		yield return new WaitForSeconds (1f);
		WiimoteManager.Cleanup (_wiimote);
		_wiimote = null;
		yield return new WaitForSeconds (1f);
		_wiimote_reset = false;
	}

	//////////////////////////////////////
	///* 終了時に呼ばれるイベント関数 *///
	//////////////////////////////////////
	void OnApplicationQuit(){
		Reset ();
	}
}
