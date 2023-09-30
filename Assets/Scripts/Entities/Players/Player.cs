namespace Entities
{
    /// <summary>
    /// Just to differentiate between Players and other entities,
    /// will eventually hold player name, etc for multiplayer
    /// </summary>
    public class Player : Entity
    {
        //Add this player to the list
        protected override void Awake()
        {
            Instances.Add(this);
        }
    }
}