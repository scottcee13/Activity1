using System;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonCrawler.Dialogue
{
    [Serializable]
    public class DialogueLine
    {
        public string speakerName;
        [TextArea(2, 5)] public string text;
    }

    [Serializable]
    public class DialogueChoice
    {
        public string choiceText;
        public int nextNodeIndex = -1;
        [Tooltip("Item granted when this choice is picked")]
        public string grantItemId;
        [Tooltip("Quest objective trigger id")]
        public string questTriggerId;
    }

    [Serializable]
    public class DialogueNode
    {
        public List<DialogueLine> lines = new List<DialogueLine>();
        public List<DialogueChoice> choices = new List<DialogueChoice>();
        public bool endAfterLines;
    }

    [CreateAssetMenu(fileName = "DialogueData", menuName = "Dungeon/Dialogue Data")]
    public class DialogueDataSO : ScriptableObject
    {
        public string dialogueId;
        public List<DialogueNode> nodes = new List<DialogueNode>();
        [Tooltip("Hint id stored when player completes this dialogue (for door puzzle)")]
        public string grantsHintId;
    }
}
