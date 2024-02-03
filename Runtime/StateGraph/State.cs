﻿#pragma warning disable 0067

using UnityEngine;

namespace Nonatomic.VSM2.StateGraph
{
	public abstract class State : ScriptableObject
	{
		public GameObject GameObject { get; set; }
		public StateMachine StateMachine { get; set; }
		
		public abstract void Enter();
		public abstract void Exit();
		
		public virtual void Awake()
		{
			//..
		}
		
		public virtual void Start()
		{
			//..
		}

		public virtual void Update()
		{
			//..
		}

		public virtual void FixedUpdate()
		{
			//..
		}

		public virtual void OnDestroy()
		{
			//..
		}
	}
}