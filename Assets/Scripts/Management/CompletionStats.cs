using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompletionStats {

    bool m_completedTutorial = false;
    bool[] m_completedTutorialSegments;

    public bool completedTutorial {
        get { return m_completedTutorial; }
        set { m_completedTutorial = value; }
    }

    public bool[] completedTutorialSegments {
        get { return m_completedTutorialSegments; }
        set { m_completedTutorialSegments = value; }
    }
}
