﻿using System;
using System.Collections.Generic;
using System.Linq;
using Vixen.Rule;
using Vixen.Sys;

namespace Vixen.Services {
	public class ChannelNodeService {
		static private ChannelNodeService _instance;

		private ChannelNodeService() {
		}

		public static ChannelNodeService Instance {
			get { return _instance ?? (_instance = new ChannelNodeService()); }
		}

		public ChannelNode CreateSingle(ChannelNode parentNode, string name = null, bool createChannel = false, int index = -1) {
			name = name ?? "Unnamed";

			ChannelNode channelNode = VixenSystem.Nodes.AddNode(name);
			VixenSystem.Nodes.AddChildToParent(channelNode, parentNode, index);

			Channel channel = createChannel ? _CreateChannel(name) : null;
			channelNode.Channel = channel;
			VixenSystem.Channels.AddChannel(channel);

			return channelNode;
		}

		public ChannelNode[] CreateMultiple(ChannelNode parentNode, int count, bool createChannel = false) {
			return Enumerable.Range(0, count).Select(x => CreateSingle(parentNode, null, createChannel)).ToArray();
		}

		public ChannelNode ImportTemplateOnce(string templateFileName, ChannelNode parentNode) {
			ChannelNodeTemplate channelNodeTemplate = ChannelNodeTemplate.Load(templateFileName);
			if(channelNodeTemplate == null) return null;

			VixenSystem.Nodes.AddChildToParent(channelNodeTemplate.ChannelNode, parentNode);

			return channelNodeTemplate.ChannelNode;
		}

		public ChannelNode[] ImportTemplateMany(string templateFileName, ChannelNode parentNode, int count) {
			return Enumerable.Range(0, count).Select(x => ImportTemplateOnce(templateFileName, parentNode)).NotNull().ToArray();
		}

		public void Rename(ChannelNode channelNode, string name) {
			channelNode.Name = name;
		}

		public void Rename(IEnumerable<ChannelNode> channelNodes, INamingRule namingRule) {
			ChannelNode[] channelNodeArray = channelNodes.ToArray();
			string[] names = namingRule.GenerateNames(channelNodeArray.Length);

			for(int i=0; i<channelNodeArray.Length; i++) {
				Rename(channelNodeArray[i], names[i]);
			}
		}

		public void Patch(ChannelNode channelNode, IPatchingRule patchingRule, bool clearExisting = false) {
			Patch(channelNode.AsEnumerable(), patchingRule, clearExisting);
		}

		public void Patch(IEnumerable<ChannelNode> channelNodes, IPatchingRule patchingRule, bool clearExisting = false) {
			// If this were instead a part of an ongoing operation involving multiple calls to a method
			// like this, the change set would have to be outside this method and passed in with the
			// caller being responsible for its commitment.
			ChannelPatchingChangeSet channelPatchingChangeSet = new ChannelPatchingChangeSet();

			_PatchUsingChangeset(channelNodes, patchingRule, channelPatchingChangeSet);
	
			channelPatchingChangeSet.Commit(clearExisting);
		}

		private void _PatchUsingChangeset(IEnumerable<ChannelNode> channelNodes, IPatchingRule patchingRule, ChannelPatchingChangeSet channelPatchingChangeSet) {
			Channel[] channelArray = channelNodes.Select(x => x.Channel).NotNull().ToArray();
			ControllerReferenceCollection[] destinations = patchingRule.GenerateControllerReferenceCollections(channelArray.Length).ToArray();

			int patchesToAdd = Math.Min(channelArray.Length, destinations.Length);
			for(int i = 0; i < patchesToAdd; i++) {
				channelPatchingChangeSet.AddChannelPatches(channelArray[i].Id, destinations[i]);
			}
		}

		private Channel _CreateChannel(string name) {
			return new Channel(name);
		}
	}
}
