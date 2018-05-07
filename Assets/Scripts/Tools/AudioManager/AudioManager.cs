#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[ExecuteInEditMode]
public class AudioManager : MonoBehaviour {

	[SerializeField]
	[HideInInspector]
	List<AudioClip> soundsEditor;

	static AudioListener listener;
	static AudioSource source;
	static AudioClip[] sounds;

	private void Awake() {
		if (!Application.isPlaying) return;
		source = GetComponent<AudioSource>();

		if (soundsEditor != null) sounds = soundsEditor.ToArray();
	}

	/// <summary>
	/// 播放一次聲音檔 (可以在 Play 的聲音還在播放下播)
	/// </summary>
	/// <param name="audio">要播放的聲音</param>
	/// <param name="volume">音量大小，最小是0, 沒有最大值</param>
	public static void PlayOneShot(Audios audio, float volume = 1) {
		int index = (int)audio;
		if (sounds == null || index >= sounds.Length) return;
		CreateAudioSource();

		if (!source.isPlaying) source.volume = 1;
		source.PlayOneShot(sounds[(int)audio], volume);
	}
	/// <summary>
	/// 播放聲音檔
	/// </summary>
	/// <param name="audio">要播放的聲音</param>
	/// <param name="volume">音量大小，最小是0, 最大是1</param>
	/// <param name="loop">是否循環播放</param>
	public static void Play(Audios audio, float volume = 1, bool loop = false) {
		int index = (int)audio;
		if (sounds == null || index >= sounds.Length) return;
		CreateAudioSource();

		source.clip = sounds[index];
		source.volume = volume;
		source.loop = loop;
		source.Play();
	}
	/// <summary>
	/// 停止播放
	/// </summary>
	public static void Stop() {
		CreateAudioSource();
		source.Stop();
	}
	/// <summary>
	/// 全遊戲靜音 (原本是開聲音的，呼叫後靜音，原本是靜音的，呼叫後開啟聲音)
	/// </summary>
	public static void Mute() {
		if (listener == null) {
			listener = FindObjectOfType<AudioListener>();
		}
		listener.enabled = !listener.enabled;
	}

	static void CreateAudioSource() {
		if (source == null) {
			source = new GameObject("AudioSource").AddComponent<AudioSource>();
		}
	}




#if UNITY_EDITOR
	string soundFilesFolder = "Assets/Artist/Sounds";                   // 聲音檔所在的資料夾
	string scriptFIleFolder = "Assets/Scripts/Tools/AudioManager";      // 聲音檔 Enum 腳本輸出位置

	[SerializeField]
	[HideInInspector]
	string[] preAudios;

	private void Update() {

		if (!Application.isEditor) return;
		if (!File.Exists(scriptFIleFolder + "/AudioEnums.cs")) {
			WriteEnumsToFile(new List<string>());
		}

		soundsEditor = new List<AudioClip>();

		List<string> audios = new List<string>();

		CreateFolder(soundFilesFolder);
		DirectoryInfo di = new DirectoryInfo(soundFilesFolder);
		foreach (FileInfo f in di.GetFiles()) {
			if (f.Extension != ".meta") {
				AudioClip a = AssetDatabase.LoadAssetAtPath<AudioClip>(soundFilesFolder + "/" + f.Name);
				if (a == null) continue;
				soundsEditor.Add(a);
				audios.Add(f.Name.Substring(0, f.Name.IndexOf('.')));
			}
		}

		bool difference = preAudios == null;             // 是否重新寫入


		if (!difference && preAudios.Length == audios.Count) {          // 如果數量相同，簡查是否每一個元素都相同
			for (int i = 0; i < audios.Count; i++) {
				if (preAudios[i] != audios[i]) {            // 只要其中一個不相同 (index 不同也算)
					difference = true;
					break;
				}
			}
		} else {                                        // 如果數量不相同就直接重新寫入
			difference = true;
		}

		if (difference) {
			CreateFolder(scriptFIleFolder);
			WriteEnumsToFile(audios);
			preAudios = new string[audios.Count];
			audios.CopyTo(preAudios);
		}
	}

	/// <summary>
	///  path 需包含 Assets
	/// </summary>
	static void CreateFolder(string path) {
		if (path == null || AssetDatabase.IsValidFolder(path)) return;

		string[] s = path.Split('/');
		if (s.Length <= 0 || s[0] != "Assets") return;

		string pathStr = "Assets";
		string pathStrNext = pathStr;
		for (int i = 1; i < s.Length; i++) {

			pathStrNext += "/" + s[i];

			if (!AssetDatabase.IsValidFolder(pathStrNext)) {
				AssetDatabase.CreateFolder(pathStr, s[i]);
			}
			pathStr = pathStrNext;
		}
	}

	void WriteEnumsToFile(List<string> enumEntries) {
		string enumName = "Audios";
		string filePathAndName = scriptFIleFolder + "/AudioEnums.cs";
		int unKnowFileCount = 0;

		using (StreamWriter streamWriter = new StreamWriter(filePathAndName)) {
			streamWriter.WriteLine("public enum " + enumName);
			streamWriter.WriteLine("{");
			for (int i = 0; i < enumEntries.Count; i++) {
				string name = RemoveSymbols(enumEntries[i]);

				if (name == "") {
					name = "UnknownFileName" + unKnowFileCount.ToString();
				}
				streamWriter.WriteLine("\t" + name + " = " + i.ToString() + ",");
			}
			streamWriter.WriteLine("}");
		}
		AssetDatabase.Refresh();

	}

	string RemoveSymbols(string str) {

		List<char> chars = new List<char>();

		foreach (char c in str) {
			if ((c >= 48 && c <= 57) || (c >= 65 && c <= 90) || (c >= 97 && c <= 122) || c > 10000)
				//print(c);
				chars.Add(c);
		}
		return new string(chars.ToArray());
	}
#endif
}
