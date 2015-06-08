using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;
using System.Collections;

namespace NobleMuffins.StagedComputationKit {
	public class StagedComputationAgent : MonoBehaviour {

		private static StagedComputationAgent instance;
		public static StagedComputationAgent Instance {
			get {
				if(instance == null) {
					var go = new GameObject();
					instance = go.AddComponent<StagedComputationAgent>();
				}
				return instance;
			}
		}

		private class JobState {
			public readonly Job job;
			public IEnumerator elementEnumerator;
			
			public JobState(Job job)
			{
				this.job = job;
			}
		}

		public long millisecondsPerFrame = 20;
		public event System.EventHandler<JobCompletionEventArgs> OnJobComplete;  

		private readonly ICollection<JobState> jobStates = new HashSet<JobState>();
		private readonly Stopwatch stopwatch = new Stopwatch();

		void Awake() {
			if(instance == null)
			{
				instance = this;
			}
			else if(instance != this)
			{
				GameObject.DestroyImmediate(this);
			}
		}

		public void AddJob(Job job) {
			var state = new JobState(job);
			jobStates.Add(state);
			if(!enabled) {
				enabled = true;
			}
		}

		public void RemoveJob(Job job) {
			var jobStatesToRemove = new List<JobState>();
			foreach(var jobState in jobStates)
			{
				if(jobState.job == job)
				{
					jobStatesToRemove.Add(jobState);
				}
			}
			foreach(var jobState in jobStatesToRemove)
			{
				jobStates.Remove(jobState);
			}
		}

		void Update () {
			if(jobStates.Count > 0)
			{
				stopwatch.Reset();
				stopwatch.Start();

				//Pick a job state – doesn't matter which

				var jobStateEnumator = jobStates.GetEnumerator();
				jobStateEnumator.MoveNext();

				var jobState = jobStateEnumator.Current;
				var job = jobState.job;
				if(jobState.elementEnumerator == null) {
					jobState.elementEnumerator = job.elements.GetEnumerator();
				}
				var elementEnumerator = jobState.elementEnumerator;
				
				bool stillHasWorkToDo = elementEnumerator.MoveNext();
				
				while(stillHasWorkToDo && (stopwatch.ElapsedMilliseconds < millisecondsPerFrame))
				{
					var element = elementEnumerator.Current;
					job.function(element);
					stillHasWorkToDo = elementEnumerator.MoveNext();
				}

				stopwatch.Stop();

				if(stillHasWorkToDo == false)
				{
					if(OnJobComplete != null) {
						OnJobComplete(this, new JobCompletionEventArgs(job));
					}

					jobStates.Remove(jobState);
				}
			}
			else
			{
				enabled = false;
			}
		}
	}
}