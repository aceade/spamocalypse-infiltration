using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class CreateSpammerVoice : EditorWindow {

	string assetName = "NewSpammerVoice";

	public List<AudioClip> patrollingSounds = new List<AudioClip>(),
		 attackingSounds = new List<AudioClip>(), 
		 searchingSounds = new List<AudioClip>(), damagedSounds = new List<AudioClip>(),
		 dyingSounds = new List<AudioClip>();

	bool showPatSounds, showAttackingSounds, showSearchingSounds, showDamagedSounds, showDyingSounds;

	AudioClip newClip;

	SpammerVoice template;

	public void CreateAsset()
	{
		SpammerVoice asset = ScriptableObject.CreateInstance<SpammerVoice>();

		AssetDatabase.CreateAsset(asset, "Assets/Sound/" + assetName + ".asset");
		asset.assetName = assetName;
		asset.attackingSounds = attackingSounds;
		asset.damagedSounds = damagedSounds;
		asset.deathSounds = dyingSounds;
		asset.searchingSounds = searchingSounds;
		asset.idleSounds = patrollingSounds;
		if (template != null)
		{
			Debug.Log("Using template");
			asset.contactSounds = template.contactSounds;
			asset.visualSuspicionSounds = template.visualSuspicionSounds;
			asset.auralSuspicionSounds = template.auralSuspicionSounds;
			asset.flameSounds = template.flameSounds;
			asset.sockpuppetSounds = template.sockpuppetSounds;
			if (attackingSounds.Count == 0)
			{
				asset.attackingSounds = template.attackingSounds;
			}
			if (damagedSounds.Count == 0)
			{
				asset.damagedSounds = template.damagedSounds;
			}
			if (dyingSounds.Count == 0)
			{
				asset.deathSounds = template.deathSounds;
			}
			if (patrollingSounds.Count == 0)
			{
				asset.idleSounds = template.idleSounds;
			}
			if (searchingSounds.Count == 0)
			{
				asset.searchingSounds = template.searchingSounds;
			}
		}
		AssetDatabase.SaveAssets();

		Selection.activeObject = asset;
	}

	[MenuItem("Assets/Create/SpammerVoice")]
	static void ShowWindow()
	{
		EditorWindow myWindow = EditorWindow.GetWindow(typeof(CreateSpammerVoice));
		myWindow.titleContent = new GUIContent("Creating new Spammer voice");
	}

	void OnGUI()
	{
		template = (SpammerVoice) EditorGUILayout.ObjectField("Template", template, typeof(SpammerVoice), true);

		assetName = GUILayout.TextField(assetName);
		showPatSounds = EditorGUILayout.Foldout(showPatSounds, "Patrolling clips");
		AddList(patrollingSounds, showPatSounds);
		showAttackingSounds = EditorGUILayout.Foldout(showAttackingSounds, "Attacking clips");
		AddList(attackingSounds, showAttackingSounds);

		showSearchingSounds = EditorGUILayout.Foldout(showSearchingSounds, "Searching clips");
		AddList(searchingSounds, showSearchingSounds);

		showDamagedSounds = EditorGUILayout.Foldout(showDamagedSounds, "Damaged sounds");
		AddList(damagedSounds, showDamagedSounds);

		showDyingSounds = EditorGUILayout.Foldout(showDyingSounds, "Dying sounds");
		AddList(dyingSounds, showDyingSounds);

		if (GUILayout.Button("Create new Voice"))
		{
			CreateAsset();
		}
	}

	void AddList (List<AudioClip> theList, bool showList)
	{
		if (showList)
		{
			if (theList.Count > 0)
			{
				for (int i = 0; i < theList.Count; i++)
				{
					EditorGUILayout.BeginHorizontal ();
					theList [i] = (AudioClip)EditorGUILayout.ObjectField (theList [i], typeof(AudioClip), true);
					if (GUILayout.Button ("Remove"))
					{
						theList.RemoveAt (i);
					}
					EditorGUILayout.EndHorizontal ();
				}

			}
			EditorGUILayout.BeginHorizontal ();
			newClip = (AudioClip)EditorGUILayout.ObjectField ("New voice clip", newClip, typeof(AudioClip), true);
			if (GUILayout.Button ("Add"))
			{
				if (newClip != null)
				{
					theList.Add (newClip);
				}
			}
			EditorGUILayout.EndHorizontal ();
			EditorGUILayout.Space ();
		}

	}
}
