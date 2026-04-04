public class QuestInstance
{
    public QuestDataSO data;
    public QuestStatus status;
    public ObjectiveBase objective;

    public QuestInstance(QuestDataSO data, QuestStatus status)
    {
        this.data = data;
        this.status = status;

        switch (data.objectiveType)
        {
            case ObjectiveType.Kill:
                objective = new KillObjective(data, status);
                break;

            case ObjectiveType.Dialogue:
                objective = new DialogueObjective(data, status);
                break;

            case ObjectiveType.Exploration:
                objective = new ExplorationObjective(data, status);
                break;
        }
    }
}