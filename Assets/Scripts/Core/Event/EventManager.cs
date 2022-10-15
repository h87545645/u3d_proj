
using System;
using System.Collections.Generic;

public static class EventCenter
{
	public delegate void EventHandle();
	public delegate void EventHandle<T>(T value);
	public delegate void EventHandle<T1, T2>(T1 value1, T2 value2);
	public delegate void EventHandle<T1, T2, T3>(T1 value1, T2 value2, T3 value3);
	static Dictionary<Game_Event, Delegate> eventHandles;

	/// <summary>
	/// �����¼�
	/// </summary>
	/// <param name="eventName">�¼���</param>
	[Obsolete("�ѹ�ʱ����ʹ��PostEvent����")]
	public static void SendEvent(Game_Event eventName)
	{
		PostEvent(eventName);
	}

	/// <summary>
	/// �����¼�
	/// </summary>
	/// <param name="eventName">�¼���</param>
	/// <param name="value">����</param>
	[Obsolete("�ѹ�ʱ����ʹ��PostEvent����")]
	public static void SendEvent(Game_Event eventName, object value)
	{
		PostEvent(eventName, value);
	}

	/// <summary>
	/// �����¼�
	/// </summary>
	/// <param name="eventName">�¼���</param>
	public static void PostEvent(Game_Event eventName)
	{
		if (eventHandles == null) return;
		Delegate d;
		if (eventHandles.TryGetValue(eventName, out d))
		{
			if (d == null) return;
			EventHandle call = d as EventHandle;
			if (call != null)
			{
				call();
			}
			else if (d is EventHandle<object>)
			{
				//����֮ǰ�ɰ汾EgoEventCenter�߼�
				EventHandle<object> call2 = d as EventHandle<object>;
				call2(null);
			}
			else
			{
				throw new Exception(string.Format("�¼�{0}�����Ų�ͬ���͵�ί��", eventName));
			}
		}
	}

	/// <summary>
	/// �����¼�
	/// </summary>
	/// <param name="eventName"></param>
	/// <param name="value"></param>
	/// <typeparam name="T"></typeparam>
	public static void PostEvent<T>(Game_Event eventName, T value)
	{
		if (eventHandles == null) return;
		Delegate d;
		if (eventHandles.TryGetValue(eventName, out d))
		{
			if (d == null) return;
			EventHandle<T> call = d as EventHandle<T>;
			if (call != null)
			{
				call(value);
			}
			else
			{
				throw new Exception(string.Format("�¼�{0}�����Ų�ͬ���͵�ί��{1}", eventName, d.GetType()));
			}
		}
	}

	/// <summary>
	/// �����¼�
	/// </summary>
	/// <param name="eventName"></param>
	/// <param name="value1"></param>
	/// <param name="value2"></param>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	public static void PostEvent<T1, T2>(Game_Event eventName, T1 value1, T2 value2)
	{
		if (eventHandles == null) return;
		Delegate d;
		if (eventHandles.TryGetValue(eventName, out d))
		{
			if (d == null) return;
			EventHandle<T1, T2> call = d as EventHandle<T1, T2>;
			if (call != null)
			{
				call(value1, value2);
			}
			else
			{
				throw new Exception(string.Format("�¼�{0}�����Ų�ͬ���͵�ί��{1}", eventName, d.GetType()));
			}
		}
	}

	/// <summary>
	/// �����¼�
	/// </summary>
	/// <param name="eventName"></param>
	/// <param name="value1"></param>
	/// <param name="value2"></param>
	/// <param name="value3"></param>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <typeparam name="T3"></typeparam>
	public static void PostEvent<T1, T2, T3>(Game_Event eventName, T1 value1, T2 value2, T3 value3)
	{
		if (eventHandles == null) return;
		Delegate d;
		if (eventHandles.TryGetValue(eventName, out d))
		{
			if (d == null) return;
			EventHandle<T1, T2, T3> call = d as EventHandle<T1, T2, T3>;
			if (call != null)
			{
				call(value1, value2, value3);
			}
			else
			{
				throw new Exception(string.Format("�¼�{0}�����Ų�ͬ���͵�ί��{1}", eventName, d.GetType()));
			}
		}
	}

	/// <summary>
	/// �����¼�
	/// </summary>
	/// <param name="eventName">�¼���</param>
	/// <param name="handle">�ص�</param>
	public static void AddListener(Game_Event eventName, EventHandle handle)
	{
		OnListeningAdd(eventName, handle);
		eventHandles[eventName] = (EventHandle)eventHandles[eventName] + handle;
	}

	/// <summary>
	/// �����¼�
	/// </summary>
	/// <param name="eventName">�¼���</param>
	/// <param name="handle">�ص�</param>
	public static void AddListener(Game_Event eventName, EventHandle<object> handle)
	{
		OnListeningAdd(eventName, handle);
		eventHandles[eventName] = (EventHandle<object>)eventHandles[eventName] + handle;
	}

	/// <summary>
	/// �����¼�
	/// </summary>
	/// <param name="eventName"></param>
	/// <param name="handle"></param>
	/// <typeparam name="T"></typeparam>
	public static void AddListener<T>(Game_Event eventName, EventHandle<T> handle)
	{
		OnListeningAdd(eventName, handle);
		eventHandles[eventName] = (EventHandle<T>)eventHandles[eventName] + handle;
	}

	/// <summary>
	/// �����¼�
	/// </summary>
	/// <param name="eventName"></param>
	/// <param name="handle"></param>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	public static void AddListener<T1, T2>(Game_Event eventName, EventHandle<T1, T2> handle)
	{
		OnListeningAdd(eventName, handle);
		eventHandles[eventName] = (EventHandle<T1, T2>)eventHandles[eventName] + handle;
	}

	/// <summary>
	/// �����¼�
	/// </summary>
	/// <param name="eventName"></param>
	/// <param name="handle"></param>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <typeparam name="T3"></typeparam>
	public static void AddListener<T1, T2, T3>(Game_Event eventName, EventHandle<T1, T2, T3> handle)
	{
		OnListeningAdd(eventName, handle);
		eventHandles[eventName] = (EventHandle<T1, T2, T3>)eventHandles[eventName] + handle;
	}

	/// <summary>
	/// �Ƴ��¼�����
	/// </summary>
	/// <param name="eventName">�¼���</param>
	/// <param name="handle">�ص�</param>
	[Obsolete("�ѹ�ʱ����ʹ��RemoveListener����")]
	public static void RemoveHandle(Game_Event eventName, EventHandle handle)
	{
		RemoveListener(eventName, handle);
	}

	/// <summary>
	/// �Ƴ��¼�����
	/// </summary>
	/// <param name="eventName">�¼���</param>
	/// <param name="handle">�ص�</param>
	[Obsolete("�ѹ�ʱ����ʹ��RemoveListener����")]
	public static void RemoveHandle(Game_Event eventName, EventHandle<object> handle)
	{
		RemoveListener<object>(eventName, handle);
	}

	/// <summary>
	/// �Ƴ��¼�����
	/// </summary>
	/// <param name="eventName">�¼���</param>
	/// <param name="handle">�ص�</param>
	public static void RemoveListener(Game_Event eventName, EventHandle handle)
	{
		if (eventHandles == null)
		{
			return;
		}
		if (!eventHandles.ContainsKey(eventName)) return;
		OnListeningRemove(eventName, handle);
		eventHandles[eventName] = (EventHandle)eventHandles[eventName] - handle;
	}

	/// <summary>
	/// �Ƴ��¼�����
	/// </summary>
	/// <param name="eventName">�¼���</param>
	/// <param name="handle">�ص�</param>
	public static void RemoveListener(Game_Event eventName, EventHandle<object> handle)
	{
		RemoveListener<object>(eventName, handle);
	}

	/// <summary>
	/// �Ƴ��¼�����
	/// </summary>
	/// <param name="eventName">�¼���</param>
	/// <param name="handle"></param>
	public static void RemoveListener<T>(Game_Event eventName, EventHandle<T> handle)
	{
		if (eventHandles == null)
		{
			return;
		}
		if (!eventHandles.ContainsKey(eventName)) return;
		OnListeningRemove(eventName, handle);
		eventHandles[eventName] = (EventHandle<T>)eventHandles[eventName] - handle;
	}

	/// <summary>
	/// �Ƴ��¼�����
	/// </summary>
	/// <param name="eventName">�¼���</param>
	/// <param name="handle"></param>
	public static void RemoveListener<T1, T2>(Game_Event eventName, EventHandle<T1, T2> handle)
	{
		if (eventHandles == null)
		{
			return;
		}
		if (!eventHandles.ContainsKey(eventName)) return;
		OnListeningRemove(eventName, handle);
		eventHandles[eventName] = (EventHandle<T1, T2>)eventHandles[eventName] - handle;
	}

	/// <summary>
	/// �Ƴ��¼�����
	/// </summary>
	/// <param name="eventName">�¼���</param>
	/// <param name="handle"></param>
	public static void RemoveListener<T1, T2, T3>(Game_Event eventName, EventHandle<T1, T2, T3> handle)
	{
		if (eventHandles == null)
		{
			return;
		}
		if (!eventHandles.ContainsKey(eventName)) return;
		OnListeningRemove(eventName, handle);
		eventHandles[eventName] = (EventHandle<T1, T2, T3>)eventHandles[eventName] - handle;
	}

	/// <summary>
	/// �Ƴ��¼�
	/// </summary>
	/// <param name="eventName"></param>
	public static void RemoveEvent(Game_Event eventName)
	{
		Internal_RemoveEvent(eventName, true);
	}

	static void Internal_RemoveEvent(Game_Event eventName, bool removeFromDic)
	{
		if (eventHandles == null)
		{
			return;
		}
		if (eventHandles.ContainsKey(eventName))
		{
			var callback = eventHandles[eventName];
			Delegate[] invokeList = callback.GetInvocationList();
			foreach (var invokeItem in invokeList)
			{
				Delegate.Remove(callback, invokeItem);
			}

			if (removeFromDic) eventHandles.Remove(eventName);
		}
	}

	static void OnListeningAdd(Game_Event eventName, Delegate callback)
	{
		if (eventHandles == null)
			eventHandles = new Dictionary<Game_Event, Delegate>();
		if (!eventHandles.ContainsKey(eventName))
		{
			eventHandles.Add(eventName, null);
		}
		Delegate d = eventHandles[eventName];
		if (d != null && d.GetType() != callback.GetType())
		{
			throw new Exception(string.Format("����������ֲ�ͬ���͵�ί��,ί��1Ϊ{0}��ί��2Ϊ{1}", d.GetType(), callback.GetType()));
		}
	}

	static void OnListeningRemove(Game_Event eventName, Delegate callback)
	{
		if (eventHandles.ContainsKey(eventName))
		{
			Delegate d = eventHandles[eventName];
			if (d != null && d.GetType() != callback.GetType())
			{
				throw new Exception(string.Format("�����Ƴ���ͬ���͵��¼����¼���{0},�Ѵ洢��ί������{1},��ǰ�¼�ί��{2}", eventName, d.GetType(), callback.GetType()));
			}
		}
		else
		{
			throw new Exception(string.Format("û���¼���{0}", eventName));
		}
	}

	/// <summary>
	/// �Ƴ������¼�
	/// </summary>
	public static void RemoveAllEvent()
	{
		if (eventHandles == null)
		{
			return;
		}
		foreach (var keyValuePair in eventHandles)
		{
			Internal_RemoveEvent(keyValuePair.Key, false);
		}
		eventHandles.Clear();
	}
}
