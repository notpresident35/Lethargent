using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CompletionStats {

    bool m_completedTutorial = false;
    // These two lists are updated separately because players might save and quit after they complete an objective but before they finish a cutscene
    bool[] m_completedTutorialSegments = new bool[Statics.TotalCutscenesCount]; // Tracks player progress as they complete each objective
    bool[] m_cutscenesWatched = new bool [Statics.TotalCutscenesCount]; // Tracks player progress after they finish each cutscene
    bool m_cutsceneIsPlaying = true;

    public bool completedTutorial {
        get { return m_completedTutorial; }
        set { m_completedTutorial = value; }
    }

    public bool cutsceneIsPlaying {
        get { return m_cutsceneIsPlaying; }
        set { m_cutsceneIsPlaying = value; }
    }

    public bool[] completedTutorialSegments {
        get { return m_completedTutorialSegments; }
        set { m_completedTutorialSegments = value; }
    }

    public bool [] cutscenesWatched {
        get { return m_cutscenesWatched; }
        set { m_cutscenesWatched = value; }
    }

    public CompletionStats () {

    }
}
