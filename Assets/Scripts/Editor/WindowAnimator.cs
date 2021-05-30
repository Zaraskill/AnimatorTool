using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WindowAnimator : EditorWindow
{

    private Animator[] animatorsInScene;
    private bool[] dropdownState;
    private Vector2 sliderPosition;

    private AnimationClip clip;
    private Animator animatorToPlay;
    private float animationSpeed = 1;
    private float minSpeed = 0.5f;
    private float maxSpeed = 10f;

    [MenuItem("Window/WindowAnimator")]
    public static void ShowWindow()
    {
        GetWindow(typeof(WindowAnimator));
    }
    private void OnEnable()
    {
        string[] fileGuidsArr = AssetDatabase.FindAssets(filter: "t:AnimatorController");
        dropdownState = new bool[fileGuidsArr.Length];
    }

    private void OnInspectorUpdate()
    {
        if(animatorToPlay)
            animatorToPlay.speed = animationSpeed;
    }

    private void OnGUI()
    {
        if (EditorApplication.isPlaying)
        {
            GUILayout.Label("Cant use the preview window in play mode", EditorStyles.boldLabel);
        }
        else
        {
            //Display Settings
            GUILayout.Label("Settings", EditorStyles.boldLabel);

            GUILayout.BeginHorizontal();
            minSpeed = EditorGUILayout.FloatField("Minimum Speed", minSpeed);
            maxSpeed = EditorGUILayout.FloatField("Maximum Speed", maxSpeed);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Animation Speed", EditorStyles.label);            
            animationSpeed = EditorGUILayout.Slider(animationSpeed, minSpeed, maxSpeed);
            GUILayout.EndHorizontal();

            //Display animations
            sliderPosition = GUILayout.BeginScrollView(sliderPosition, false, false);

            animatorsInScene = FindObjectsOfType<Animator>();

            for (int i = 0; i < animatorsInScene.Length; i++)
            {

                if (EditorGUILayout.DropdownButton(new GUIContent(animatorsInScene[i].runtimeAnimatorController.name), FocusType.Keyboard))
                {
                    dropdownState[i] = !dropdownState[i];
                    if (dropdownState[i])
                    {
                        Selection.activeObject = animatorsInScene[i];
                    }
                    else
                    {
                        Selection.activeObject = null;
                    }
                    
                }

                if (dropdownState[i])
                {
                    for (int j = 0; j < animatorsInScene[i].runtimeAnimatorController.animationClips.Length; j++)
                    {
                        GUILayout.BeginHorizontal();

                        GUILayout.Label(animatorsInScene[i].runtimeAnimatorController.animationClips[j].name, EditorStyles.label);

                        if (GUILayout.Button("Play", GUILayout.ExpandWidth(false)))
                        {
                            clip = animatorsInScene[i].runtimeAnimatorController.animationClips[j];
                            animatorToPlay = animatorsInScene[i];

                            ResetAnimation();

                            EditorApplication.update -= PlayAnimation;
                            EditorApplication.update += PlayAnimation;
                        }

                        if (GUILayout.Button("Stop", GUILayout.ExpandWidth(false)))
                        {
                            ResetAnimation();
                            EditorApplication.update -= PlayAnimation;
                        }

                        GUILayout.EndHorizontal();
                    }
                }
            }

            GUILayout.EndScrollView();
        }        
    }


    private void PlayAnimation()
    {
        clip.SampleAnimation(animatorToPlay.gameObject, Time.deltaTime);
        animatorToPlay.Update(Time.deltaTime);
    }

    private void ResetAnimation()
    {
        if (!clip)
            return;

        clip.SampleAnimation(animatorToPlay.gameObject, 0);
        animatorToPlay.Play(clip.name, 0, 0f);
        animatorToPlay.Update(0);
    }
}
