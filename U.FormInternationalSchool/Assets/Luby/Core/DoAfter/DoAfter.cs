using System;
using System.Collections;
using System.Collections.Generic;
using LubyLib.Core.Singletons;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LubyLib.Core.DoAfter
{
    public class DoAfter : SpawnableSingletonProtected<DoAfter>
    {
	    protected override bool DestroyOnLoad => false;

	    private Dictionary<string, Coroutine> _coroutinesToBeStopped = new Dictionary<string, Coroutine>();
	    

	    private void Start()
	    {
		    SceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;
	    }

	    private void SceneManagerOnActiveSceneChanged(Scene old, Scene next)
	    {
		    foreach (var coroutine in _coroutinesToBeStopped.Values)
		    {
			    StopCoroutine(coroutine);
		    }
		    
		    _coroutinesToBeStopped = new Dictionary<string, Coroutine>();
	    }

	    /// <summary>
	    ///     <para>Perform an action after the duration.</para>
	    /// </summary>
	    /// <param name="duration">Time to wait in seconds.</param>
	    /// <param name="action">Action to perform. Parameter is the real elapsed time.</param>
	    /// <param name="stopOnSceneChange">Should the action be stopped if a scene changes while it is awaiting.</param>
	    public static void Seconds(float duration, Action<float> action, bool stopOnSceneChange = true) => Instance._Seconds(duration, action, stopOnSceneChange);
	    
	    private void _Seconds(float duration, Action<float> action, bool stopOnSceneChange = true)
	    {
		    if (stopOnSceneChange)
		    {
			    string key = UniqueIDsManager.GetUniqueID("DoAfterSecond_");
			    var coroutine = StartCoroutine(SecondsCoroutineNonPersistent(duration, action, key));
			    _coroutinesToBeStopped.Add(key,coroutine);
		    }
		    else
		    {
			    StartCoroutine(SecondsCoroutinePersistent(duration, action));
		    }
	    }

        private IEnumerator SecondsCoroutineNonPersistent(float duration, Action<float> action, string key)
	    {
		    float time = Time.time;
		    yield return new WaitForSeconds(Mathf.RoundToInt(duration));
		    time = Time.time - time;
		    action?.Invoke(time);
		    if (!_coroutinesToBeStopped.Remove(key))
		    {
			    Debug.LogError("Coroutine not found in dictionary! Something is wrong!");
		    }
	    }
	    
	    private IEnumerator SecondsCoroutinePersistent(float duration, Action<float> action)
	    {
		    float time = Time.time;
		    yield return new WaitForSeconds(Mathf.RoundToInt(duration));
		    time = Time.time - time;
		    action?.Invoke(time);
	    }


	    /// <summary>
	    ///     <para>Perform an action after frames.</para>
	    /// </summary>
	    /// <param name="frames">Frames to wait.</param>
	    /// <param name="action">Action to perform. Parameter is the real elapsed time.</param>
	    /// <param name="stopOnSceneChange">Should the action be stopped if a scene changes while it is awaiting.</param>
	    public static void Frames(int frames, Action<float> action, bool stopOnSceneChange = true) => Instance._Frames(frames, action, stopOnSceneChange);

        
        private void _Frames(int frames, Action<float> action, bool stopOnSceneChange = true)
        {
	        if (stopOnSceneChange)
	        {
		        string key = UniqueIDsManager.GetUniqueID("DoAfterSecond_");
		        var coroutine = StartCoroutine(Instance.FramesCoroutineNonPersistent(frames, action, key));
		        _coroutinesToBeStopped.Add(key,coroutine);
	        }
	        else
	        {
		        Instance.StartCoroutine(Instance.FramesCoroutinePersistent(frames, action));;
	        }
        }
        
        private IEnumerator FramesCoroutineNonPersistent(int frames, Action<float> action, string key)
        {
	        float time = Time.time;
	        for (int i = 0; i < frames; i++)
	        {
		        yield return null;
	        }
	        time = Time.time - time;
	        action?.Invoke(time);
	        if (!_coroutinesToBeStopped.Remove(key))
	        {
		        Debug.LogError("Coroutine not found in dictionary! Something is wrong!");
	        }
        }
	    
        private IEnumerator FramesCoroutinePersistent(int frames, Action<float> action)
        {
	        float time = Time.time;
	        for (int i = 0; i < frames; i++)
	        {
		        yield return null;
	        }
	        time = Time.time - time;
	        action?.Invoke(time);
        }
    }

}