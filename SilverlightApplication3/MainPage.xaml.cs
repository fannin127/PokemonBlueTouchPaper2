using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Linq;


namespace SilverlightApplication3
{
    public partial class MainPage : UserControl
    {

        #region GlobalVariables
        #region RouteVariables
        private Route r1;
        private Route r2;
        private Route pokeCentreR2;
        private Route oakLab;
        private Route TurnTown;
        private Route PokeCentreTurn;
        private Route pokeMartTurn;
        private Route GymTurn;
        private RouteName currentPokeCentre;

        private Route currentRoute;
        #endregion

        #region BattleVariables
        private bool fightButtonPressed;
        private Pokemon[] party;
        private List<Pokemon> pc;
        private bool lost;
        private bool inBattle;
        private BattleController battleController;
        private bool deletingAMove;
        private Pokemon deletingAMoveFrom;
        private string replacingMove;
        #endregion

        #region InteractionsAndNPC
        private NurseItem nurseJoy;
        private Interaction currentInteraction;
        NPCInteraction nurseForced;
        private bool moretoDo;

        NPC trainer;
        NPC profOak;
        NPC gymTurn1Trainer;
        NPC gymLeaderTurn;
        #endregion

        #region BagAndItems
        private Bag myBag;
        private ItemType currentBagOpen;
        private Item currentItem;
        private bool usingItem;
        private Item buyingItem;
        public int money = 1000;
        #endregion

        #region FrontEndVariables
        const int blockSize = 40;
        private int playerLeft, playerTop = 0;
        #endregion

        private bool spokeToOak;
        private int currentLookingPartyPokemon = 0;
        HashSet<int> PokesSeenID;

        #region Baadges
        private bool badge1 = false;
        private bool badge2 = false;
        private bool badge3 = false;
        private bool badge4 = false;
        private bool badge5 = false;
        private bool badge6 = false;
        private bool badge7 = false;
        private bool badge8 = false;
        #endregion

        #region Trainer
        private enum gender { boy, girl };
        private gender trainerGender;
        private string trainerName;
        #endregion

        #endregion

        public MainPage()
        {
            InitializeComponent();
            this.AddHandler(Control.KeyDownEvent, new KeyEventHandler(OnKeyDown), true);
            this.Focus();

            InitialiseVariables();

            cleanAndDraw(oakLab);
            Canvas.SetZIndex(YesNoCanvas, 100);
            Canvas.SetZIndex(Player, 100);
            Canvas.SetZIndex(PokemonInfoScreen, 200);
        }

        private void InitialiseVariables()
        {
            pc = new List<Pokemon>();
            myBag = new Bag();
            currentBagOpen = ItemType.HealAndStatus;
            PokesSeenID = new HashSet<int>();
            currentPokeCentre = RouteName.PokeCentreR2;


            nurseJoy = ItemStore.Instance.get(ItemName.Nurse) as NurseItem;

            party = new Pokemon[6];

            routeSetup();

            inBattle = false;
        }

        private void routeSetup()
        {
            nurseForced = new NPCInteraction("Now healing your pokemon. We hope to see you again!", nurseJoy, () => nurseJoy.heal(party));
            NPCInteraction nurseInteraction = new NPCInteraction("Would you like me to heal your pokeballs?", nurseJoy, () => nurseJoy.heal(party));
            NPC nj = new NPC(-4, 0, nurseInteraction, "nurseJoy", Colors.Magenta);
            MoveRoute leave = new MoveRoute(4, 0, null, "leave", Colors.Black, RouteName.Route2, -4, -4);
            NPCInteraction pcInter = new NPCInteraction("Move Pokemon...", () =>
            {
                foreach (Pokemon p in party)
                {
                    if (p != null)
                    {
                        partyPokemonLst.Items.Add(p);
                    }
                }

                foreach (Pokemon p in pc)
                {
                    pcPokemoLstn.Items.Add(p);
                }


                pcScreenCanvas.Visibility = Visibility.Visible;
                return false;
            }
            );
            NPC PcNPC = new NPC(-4, 4, pcInter, "pc", Colors.Gray);
            pokeCentreR2 = new Route(new List<InteractableObject> { nj, leave, PcNPC });
            pokeCentreR2.addVoids();

            List<InteractableObject> g2 = new List<InteractableObject>();
            g2.Add(new MoveRoute(-4, -4, null, "pokeCentreR2", Colors.Magenta, RouteName.PokeCentreR2, 4, 0));

            for (int i = (-1); i < (4); i++)
            {
                for (int j = -4; j < (4); j++)
                {
                    g2.Add(new Grass(j, i, null, "g" + i + j, Colors.Green, Pokebuilder.Instance));
                }
            }
            g2.Add(new MoveRoute(-4, 4, null, "TurnTown", Colors.Black, RouteName.TurnTown, 4, 4));


            NPCInteraction trainerInter = new NPCInteraction("Hey! Are you ready to battle?", invokeBattle);
            trainerInter.Forced = true;
            trainerInter.foeParty = new List<int> { -999 };
            trainerInter.foeLevels = new List<int> { 3, 3 };
            trainer = new NPC(1, (-2), trainerInter, "Trainer", Colors.Cyan);
            trainer.facing = Route.direction.West;
            trainer.InvokeAmountSteps = 2;
            g2.Add(trainer);
            g2.Add(new MoveRoute(4, -1, null, "movetoR1", Colors.Black, RouteName.Route1, 4, -1));

            r2 = new Route(g2, new List<int> { 4 }, 5, 2);
            r2.addVoids();
            List<InteractableObject> g = new List<InteractableObject>();

            g.Add(new MoveRoute(4, -1, null, "move", Colors.Black, RouteName.Route2, 4, -1));
            g.Add(new MoveRoute(-4, 4, null, "moveToOak", Colors.Brown, RouteName.OakLab, 5, -1));


            for (int i = -4; i < -1; i++)
            {
                for (int j = -4; j < 4; j++)
                {
                    g.Add(new Grass(j, i, null, "g" + i + j, Colors.Green, Pokebuilder.Instance));
                }
            }
            r1 = new Route(g, new List<int> { 150 }, 50, 48);
            r1.addVoids();
            spokeToOak = false;

            NPC bulbBall = null;
            NPC charBall = null;
            NPC squirBall = null;

            NPCInteraction oakInter = new NPCInteraction("Welcome to my lab, please choose a pokemon to start your journey!", () =>
            {
                spokeToOak = true;
                bulbBall.IsEnabled = true;
                charBall.IsEnabled = true;
                squirBall.IsEnabled = true;
                cleanAndDraw(oakLab);
                return spokeToOak;

            });
            oakInter.Forced = true;
            profOak = new NPC(-4, -1, oakInter, "profOak", Colors.Brown);
            profOak.addInteraction(new NPCInteraction("How is your pokedex coming along?"));

            NPCInteraction pokeBallPick = new NPCInteraction("Hope", () => { moretoDo = false; closeYesNo(); return false; });

            NPCInteraction bulbInter = new NPCInteraction("The grass pokemon, Bulbasaur", Pokebuilder.Instance.pokesForTrainer(new List<int> { 1 }, new List<int> { 5 }).First(), party, () =>
            {
                moretoDo = true;
                openYesNo("Congratulations, you have chosen Bulbasaur as your partner, have a great journey!", true);
                currentInteraction = pokeBallPick;
                bulbBall.IsEnabled = false;
                charBall.IsEnabled = false;
                squirBall.IsEnabled = false;
                return false;
            }
            );
            bulbBall = new NPC(-3, -2, bulbInter, "bulbBall", Colors.Red);
            bulbBall.IsEnabled = false;
            NPCInteraction charInter = new NPCInteraction("The fire pokemon, Charmander", Pokebuilder.Instance.pokesForTrainer(new List<int> { 4 }, new List<int> { 5 }).First(), party, () =>
            {
                moretoDo = true;
                openYesNo("Congratulations, you have chosen Charmander as your partner, have a great journey!", true);
                currentInteraction = pokeBallPick;
                bulbBall.IsEnabled = false;
                charBall.IsEnabled = false;
                squirBall.IsEnabled = false;
                return false;
            }
            );
            charBall = new NPC(-3, -1, charInter, "charBall", Colors.Red);
            charBall.IsEnabled = false;
            NPCInteraction squirInter = new NPCInteraction("The water pokemon, Squirtle", Pokebuilder.Instance.pokesForTrainer(new List<int> { 7 }, new List<int> { 5 }).First(), party, () =>
            {
                moretoDo = true;
                openYesNo("Congratulations, you have chosen Squirtle as your partner, have a great journey!", true);
                currentInteraction = pokeBallPick;
                bulbBall.IsEnabled = false;
                charBall.IsEnabled = false;
                squirBall.IsEnabled = false;
                return false;
            }
            );
            squirBall = new NPC(-3, 0, squirInter, "squirInter", Colors.Red);


            squirBall.IsEnabled = false;
            bulbBall.CanMoveOver = false;
            charBall.CanMoveOver = false;
            squirBall.CanMoveOver = false;
            profOak.CanMoveOver = false;
            MoveRoute oakLeave = new MoveRoute(5, -1, null, "move", Colors.Black, RouteName.Route1, -4, 4);

            oakLab = new Route(new List<InteractableObject> { bulbBall, charBall, squirBall, profOak, oakLeave }, -5, 5, -5, 5);
            oakLab.addVoids();

            List<InteractableObject> tList = new List<InteractableObject>();
            tList.Add(new MoveRoute(4, 4, null, "route2", Colors.Black, RouteName.Route2, -4, 4));
            tList.Add(new DeadSpace(-3, -3, null, "pokeCentreDeadSpace", Colors.Magenta, false, 3, 3));
            tList.Add(new MoveRoute(-1, -2, null, "goToCentre", Colors.Red, RouteName.PokeCentreTurnTown, 4, 0));
            tList.Add(new MoveRoute(-1, 2, null, "goToMart", Colors.Blue, RouteName.PokeMartTurnTown, 0, -1));
            tList.Add(new MoveRoute(3, -3, null, "gotToGym", Colors.Purple, RouteName.GymTurnTown, 6, 0));
            TurnTown = new Route(tList);

            TurnTown.addVoids();

            PokeCentreTurn = new Route(new List<InteractableObject> { nj, PcNPC, new MoveRoute(4, 0, null, "leave", Colors.Black, RouteName.TurnTown, -1, -2) });
            PokeCentreTurn.addVoids();
            List<Item> itemsToBuyturn = new List<Item> { ItemStore.Instance.get(ItemName.PokeBall), ItemStore.Instance.get(ItemName.Potion) };

            NPCInteraction martInter = new NPCInteraction("What would you like to buy?", () =>
            {
                BuyingCanvas.Visibility = Visibility.Visible;
                RouteCanvas.Visibility = Visibility.Collapsed;

                moneyTextBox.Text = "Money: " + money;

                itemBoxBuying.Items.Clear();
                foreach (Item i in itemsToBuyturn)
                {
                    itemBoxBuying.Items.Add(i);
                }


                return true;
            }, () =>
            {
                BagCanvas.Visibility = Visibility.Visible;
                useButton.Content = "Sell";
                MoneyInBagTextBlock.Text = "Money: " + money;
                BagSellQUp.Visibility = Visibility.Visible;
                BagSellQDown.Visibility = Visibility.Visible;
                KeyItemsButton.IsEnabled = false;
                RouteCanvas.Visibility = Visibility.Collapsed;
                return false;
            }
            );
            martInter.yesText = "Buying";
            martInter.noText = "Selling";
            martInter.closeable = true;
            NPC martPerson = new NPC(-3, -3, martInter, "martPerson", Colors.Blue);
            pokeMartTurn = new Route(new List<InteractableObject> { martPerson, new MoveRoute(0, -1, null, "leave", Colors.Black, RouteName.TurnTown, -1, 2) }, -3, 3, -3, 0);
            pokeMartTurn.addVoids();


            NPCInteraction gymTurn1Inter = new NPCInteraction("Hey! You think you can beat the leader? You need to beat me first!", invokeBattle);
            gymTurn1Inter.Forced = true;
            gymTurn1Inter.foeParty = new List<int> { 16 };
            gymTurn1Inter.foeLevels = new List<int> { 8 };
            gymTurn1Trainer = new NPC(4, -2, gymTurn1Inter, "gymTurn1", Colors.Cyan);

            NPCInteraction gymLeaderTurnInter = new NPCInteraction("Well done for getting this far. If you can beat me you will earn your first badge in this region!", invokeBattle);
            gymLeaderTurnInter.nextInter = new NPCInteraction("Well done, it was a close battle! Here take this badge!", () => { badge1 = true; return badge1; });
            gymLeaderTurnInter.Forced = true;
            gymLeaderTurnInter.foeParty = new List<int> { 16 };
            gymLeaderTurnInter.foeLevels = new List<int> { 8 };
            gymLeaderTurn = new NPC(-4, 0, gymLeaderTurnInter, "gymTurnLeader", Colors.Cyan);
            MoveRoute leaveGymTurn = new MoveRoute(6, 0, null, "leaveGymTurn", Colors.Black, RouteName.TurnTown, 3, -3);


            GymTurn = new Route(new List<InteractableObject> { gymTurn1Trainer, gymLeaderTurn, leaveGymTurn }, -2, 2, -6, 6);
            GymTurn.addVoids();

            RouteStore.Instance.Add(RouteName.Route1, r1);
            RouteStore.Instance.Add(RouteName.Route2, r2);
            RouteStore.Instance.Add(RouteName.PokeCentreR2, pokeCentreR2);
            RouteStore.Instance.Add(RouteName.OakLab, oakLab);
            RouteStore.Instance.Add(RouteName.TurnTown, TurnTown);
            RouteStore.Instance.Add(RouteName.PokeCentreTurnTown, PokeCentreTurn);
            RouteStore.Instance.Add(RouteName.PokeMartTurnTown, pokeMartTurn);
            RouteStore.Instance.Add(RouteName.GymTurnTown, GymTurn);

            myBag.Add(ItemStore.Instance.get(ItemName.MasterBall));

        }
        private void cleanAndDraw(Route n)
        {
            if (currentRoute != null)
            {
                RouteCanvas.Children.Clear();
                RouteCanvas.Children.Add(Background);
                RouteCanvas.Children.Add(Player);
            }
            currentRoute = n;
            foreach (var o in currentRoute.Draw(playerLeft, playerTop))
            {

                RouteCanvas.Children.Add(o);

            }


            if (currentRoute.contains("nurseJoy"))
            {
                currentPokeCentre = RouteStore.Instance.Get(currentRoute);
                if (!moreAlive())
                {
                    if (playerTop != -3 && playerLeft != 0)
                    {
                        playerTop = -3;
                        playerLeft = 0;
                        cleanAndDraw(currentRoute);
                    }
                    else
                    {
                        currentRoute.facing = Route.direction.North;
                        currentInteraction = nurseForced;
                        openYesNo(currentInteraction.Text, true);
                        currentInteraction.Interact();
                    }



                }
            }

        }

        private bool moreAlive()
        {
            foreach (Pokemon p in party)
            {
                if (p != null)
                {
                    if (p.CurrentHP > 0)
                    {
                        return true;
                    }
                }

            }

            return false;
        }

        public void OnKeyDown(Object sender, KeyEventArgs e)
        {
            double move = Player.ActualHeight;

            if (!inBattle)
            {
                if (e.Key.Equals(Key.L))
                {
                    if (StartCanvas.Visibility == Visibility.Collapsed)
                    {
                        StartCanvas.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        StartCanvas.Visibility = Visibility.Collapsed;
                    }
                }
                if (StartCanvas.Visibility == Visibility.Collapsed)
                {
                    if (e.Key.Equals(Key.W))
                    {
                        if (currentRoute.checkLegalMove(playerTop, playerLeft, e.Key))

                            --playerTop;
                        PerformMoveAction(currentRoute.PerformMove(playerTop, playerLeft));
                        cleanAndDraw(currentRoute);

                    }
                    else if (e.Key.Equals(Key.S))
                    {
                        if (currentRoute.checkLegalMove(playerTop, playerLeft, e.Key))

                            ++playerTop;
                        PerformMoveAction(currentRoute.PerformMove(playerTop, playerLeft));
                        cleanAndDraw(currentRoute);

                    }
                    else if (e.Key.Equals(Key.A))
                    {
                        if (currentRoute.checkLegalMove(playerTop, playerLeft, e.Key))

                            --playerLeft;
                        PerformMoveAction(currentRoute.PerformMove(playerTop, playerLeft));
                        cleanAndDraw(currentRoute);

                    }
                    else if (e.Key.Equals(Key.D))
                    {
                        if (currentRoute.checkLegalMove(playerTop, playerLeft, e.Key))

                            ++playerLeft;
                        PerformMoveAction(currentRoute.PerformMove(playerTop, playerLeft));
                        cleanAndDraw(currentRoute);

                    }
                    else if (e.Key.Equals(Key.X))
                    {
                        if (EvolveCanvas.Visibility == Visibility.Visible)
                        {
                            RouteCanvas.Visibility = Visibility.Visible;
                            EvolveCanvas.Visibility = Visibility.Collapsed;
                        }
                        else if (BattleCanvas.Visibility != Visibility.Visible)
                        {

                            Interaction inter = currentRoute.APressed(playerTop, playerLeft);
                            if (inter != null)
                            {
                                moretoDo = false;
                                currentInteraction = inter;
                                openYesNo(currentInteraction.Text, currentInteraction.yesText, currentInteraction.noText, currentInteraction.Forced, currentInteraction.closeable);
                            }
                        }
                    }

                }

            }
            else
            {
                if (e.Key.Equals(Key.X))
                {
                    var t = textBlockBat.Text;
                    if (battleController.battleQueue.Count > 0)
                    {
                        battleController.battleQueue.Dequeue().Invoke();
                    }
                }

            }
        }

        private void PerformMoveAction(OnMoveEventInfo v)
        {
            if (v != null)
            {
                if (v.Encounter != null && (battleController == null || battleController.finalMoveFinished))
                {
                    textBlock.Text = "Encounter " + v.Encounter.Name;
                    b4.IsEnabled = true;

                    battleMode(new List<Pokemon> { v.Encounter }, true);
                }
                else if (v.inter != null)
                {
                    moretoDo = false;
                    currentInteraction = v.inter;
                    openYesNo(currentInteraction.Text, currentInteraction.yesText, currentInteraction.noText, currentInteraction.Forced, currentInteraction.closeable);
                }
                else if (v.NewRoute != null) // look at
                {
                    textBlock.Text = v.NewRoute.ToString();
                    playerLeft = v.PlayerLocLeft;
                    playerTop = v.PlayerLocTop;
                    cleanAndDraw(RouteStore.Instance.Get(v.NewRoute));

                }
            }

        }

        private bool invokeBattle()
        {
            var d = (currentInteraction as NPCInteraction);
            //for testing only
            if (d.foeParty.First() == -999)
            {
                battleMode(Pokebuilder.Instance.SpecificPokesForTrainer(), false);
            }
            else
            {
                battleMode(Pokebuilder.Instance.pokesForTrainer(d.foeParty, d.foeLevels), false);
            }

            battleController.isWildBattle = false;
            b4.IsEnabled = false;
            return true;
        }
        private void battleMode(List<Pokemon> foe, bool isWild)
        {
            battleController = new BattleController(foe, party);
            battleController.isWildBattle = isWild;
            MyStatusLabel.Content = "";
            FoeStatusLabel.Content = "";
            PokesSeenID.Add(foe.First().Number);

            MySprite.Source = new BitmapImage(new Uri(("/images/back/" + party[0].Number + ".png"), UriKind.Relative));
            FoeSprite.Source = new BitmapImage(new Uri(("/images/front/" + foe.First().Number + ".png"), UriKind.Relative));

            textBlockBat.Text = "";

            inBattle = true;
            fightButtonPressed = false;
            usingItem = false;
            currentBagOpen = ItemType.HealAndStatus;
            currentItem = null;

            RouteCanvas.Visibility = Visibility.Collapsed;
            BattleCanvas.Visibility = Visibility.Visible;

            b1.Content = "Fight";
            b2.Content = "Bag";
            b3.Content = "Pokemon";
            b4.Content = "Run";

            b1.IsEnabled = true;
            b2.IsEnabled = true;
            b3.IsEnabled = true;
            b4.IsEnabled = true;

            dealWithDamage();
        }

        public void dealWithDamage()
        {
            if (inBattle)
            {
                Dictionary<string, string> conRes = battleController.DealWithDamage();

                //set labels from battlecontroller
                MyStatusLabel.Content = conRes["MyStatus"];
                FoeStatusLabel.Content = conRes["FoeStatus"];
                MyNameLabel.Content = conRes["MyName"];
                FoeNameLabel.Content = conRes["FoeName"];

                MyHPLabel.Content = conRes["MyHP"];
                FoeHPLabel.Content = conRes["FoeHP"];

                //status bar
                int iWidth = 99;
                MyHealth.Width = battleController.getHPBarWidth(battleController.CurrentInBattle) * iWidth;
                FoeHealth.Width = battleController.getHPBarWidth(battleController.CurrentFoe) * iWidth;
                MyExp.Width = battleController.getExpBarWidth();

                MyHealth.Fill = battleController.getHealthColor(battleController.CurrentInBattle);
                FoeHealth.Fill = battleController.getHealthColor(battleController.CurrentFoe);

                //return to selecting a move
                if (battleController.battleQueue.Count == 0)
                {
                    b1.Content = "Fight";
                    b2.Content = "Bag";
                    b3.Content = "Pokemon";
                    b4.Content = "Run";
                    b1.IsEnabled = true;
                    b2.IsEnabled = true;
                    b3.IsEnabled = true;
                    b4.IsEnabled = battleController.isWildBattle;
                    fightButtonPressed = false;

                }

                checkAlive();

            }

        }

        private int nextSpace()
        {
            int i = 0;
            foreach (Pokemon p in party)
            {
                if (p == null)
                {
                    return i;
                }
                ++i;
            }
            return i;
        }

        private void evolveScreen(Pokemon p)
        {
            int loc = Array.IndexOf(party, p);
            string oldName = p.Name;
            p = Pokebuilder.Instance.evolvePokemon(p);
            party[loc] = p;

            RouteCanvas.Visibility = Visibility.Collapsed;
            EvolveCanvas.Visibility = Visibility.Visible;
            EvolveText.Text = "Congratulations! Your " + oldName + " has evolved into " + p.Name + "!";
        }

        private void b1_Click(object sender, RoutedEventArgs e)
        {
            if (deletingAMove)
            {
                deleteMove(0);
                return;
            }
            b4.IsEnabled = true;
            bBack.Visibility = Visibility.Visible;

            if (!fightButtonPressed)
            {
                if (battleController.CurrentInBattle.hasPP())
                {
                    fightButtonPressed = true;
                    string[] str = battleController.getButtonContent();
                    b1.Content = str[0];
                    b2.Content = str[1];
                    b3.Content = str[2];
                    b4.Content = str[3];

                    b1.IsEnabled = battleController.CurrentInBattle.PP[0] > 0;
                    b2.IsEnabled = battleController.CurrentInBattle.PP[1] > 0;
                    b3.IsEnabled = battleController.CurrentInBattle.PP[2] > 0;
                    b4.IsEnabled = battleController.CurrentInBattle.PP[3] > 0;
                }
                else
                {
                    battleMoveSelected(new BattleAction(MyAttack, "Struggle", "Struggle"));
                }

            }
            else
            {
                battleMoveSelected(new BattleAction(MyAttack, battleController.CurrentInBattle.PerformMove(0), battleController.CurrentInBattle.Name + " used " + battleController.CurrentInBattle.PerformMove(0).ToString()));
            }
        }

        private void b2_Click(object sender, RoutedEventArgs e)
        {
            if (deletingAMove)
            {
                deleteMove(1);
                return;
            }

            if (!fightButtonPressed)
            {
                BagCanvas.Visibility = Visibility.Visible;
                useButton.Content = "Use";
                MoneyInBagTextBlock.Text = "Money: " + money;
                BagSellQDown.Visibility = Visibility.Collapsed;
                BagSellQUp.Visibility = Visibility.Collapsed;
                KeyItemsButton.IsEnabled = true;
            }
            else
            {
                battleMoveSelected(new BattleAction(MyAttack, battleController.CurrentInBattle.PerformMove(1), battleController.CurrentInBattle.Name + " used " + battleController.CurrentInBattle.PerformMove(1).ToString()));
            }
        }

        private void b3_Click(object sender, RoutedEventArgs e)
        {
            if (deletingAMove)
            {
                deleteMove(2);
                return;
            }
            if (!fightButtonPressed)
            {
                switchScreen(false);
            }
            else
            {
                battleMoveSelected(new BattleAction(MyAttack, battleController.CurrentInBattle.PerformMove(2), battleController.CurrentInBattle.Name + " used " + battleController.CurrentInBattle.PerformMove(2).ToString()));

            }
        }

        private void b4_Click(object sender, RoutedEventArgs e)
        {
            if (deletingAMove)
            {
                deleteMove(3);
                return;
            }
            if (!fightButtonPressed)
            {
                //run implement
                battleEnd("You ran away safely...");
            }
            else
            {
                battleMoveSelected(new BattleAction(MyAttack, battleController.CurrentInBattle.PerformMove(3), battleController.CurrentInBattle.Name + " used " + battleController.CurrentInBattle.PerformMove(3).ToString()));
            }
        }

        private MoveDictionary FoesNextAttack()
        {
            bool f = false;
            Random r = new Random();
            MoveDictionary foeMove = MoveStore.Instance.get("Struggle");
            int i = 0;

            if (battleController.CurrentFoe.hasPP())
            {
                while (f == false)
                {
                    i = r.Next(0, 3);
                    if (battleController.CurrentFoe.PP[i] > 0 && battleController.CurrentFoe.Moves[i] != null)
                    {
                        f = true;
                        foeMove = MoveStore.Instance.get(battleController.CurrentFoe.Moves[i]);

                    }
                }
            }

            battleController.FoeNextAttack = foeMove;
            battleController.FoeNextAttackI = i;
            return foeMove;
        }

        private string FoeAttack()
        {
            if (inBattle && !battleController.finalMoveFinished)
            {
                if (battleController.FoeNextAttack.ToFoe)
                {
                    return dealWithAttack(battleController.generalAttack(battleController.CurrentFoe, battleController.CurrentInBattle, battleController.FoeNextAttack, battleController.FoeNextAttackI, "The foe"));
                }
                else
                {
                    return dealWithAttack(battleController.generalAttack(battleController.CurrentFoe, battleController.CurrentFoe, battleController.FoeNextAttack, battleController.FoeNextAttackI, "The foe"));
                }

            }
            return "";
        }

        public string dealWithAttack(Dictionary<string, string> result)
        {
            if (result.Keys.Contains("END_BATTLE_NOW"))
            {
                battleEnd("The battle has ended!");
                return "The battle has ended!";
            }

            dealWithDamage();
            try
            {
                MyStatusLabel.Content = result["MyStatusLabel"];
            }
            catch
            {

            }
            try
            {
                FoeStatusLabel.Content = result["FoeStatusLabel"];
            }
            catch
            {

            }

            return result["return"];
        }

        private string MyAttack(string m)
        {
            if (inBattle && !battleController.finalMoveFinished)
            {
                MoveDictionary mov = MoveStore.Instance.get(m);
                if (mov.ToFoe)
                {
                    return dealWithAttack(battleController.generalAttack(battleController.CurrentInBattle, battleController.CurrentFoe, mov, Array.IndexOf(battleController.CurrentInBattle.Moves, m), ""));
                }
                else
                {
                    return dealWithAttack(battleController.generalAttack(battleController.CurrentInBattle, battleController.CurrentInBattle, mov, Array.IndexOf(battleController.CurrentInBattle.Moves, m), ""));
                }

            }

            return "";


        }

        public void deleteMove(int i)
        {
            deletingAMove = false;
            deletingAMoveFrom.Moves[i] = replacingMove;
            textBlockBat.Text = deletingAMoveFrom.Name + " learned " + replacingMove;
        }

        private void battleEnd(string text)
        {
            b1.IsEnabled = false;
            b2.IsEnabled = false;
            b3.IsEnabled = false;
            b4.IsEnabled = false;

            usingItem = false;

            battleController.finalMoveFinished = true;

            battleController.battleQueue.Enqueue(() =>
            {
                textBlockBat.Text = text;
            });

            battleController.battleQueue.Enqueue(() =>
            {
                BattleCanvas.Visibility = Visibility.Collapsed;
                RouteCanvas.Visibility = Visibility.Visible;
                inBattle = false;


                if (lost)
                {
                    cleanAndDraw(RouteStore.Instance.Get(currentPokeCentre));
                }
                else
                {
                    foreach (Pokemon p in battleController.pokesThatBattled)
                    {
                        if (p.needToEvolve())
                        {
                            evolveScreen(p);
                        }
                    }
                    try
                    {
                        if ((currentInteraction as NPCInteraction).nextInter != null)
                        {
                            currentInteraction = (currentInteraction as NPCInteraction).nextInter;
                            currentInteraction.Interact();
                            openYesNo(currentInteraction.Text, true);
                        }
                    }
                    catch { };
                }
            });



        }

        private void switchScreen(Func<Pokemon, bool> pAct)
        {
            Button[] buttons = new Button[] { s1, s2, s3, s4, s5, s6 };
            switchScreen(false);

            for (int i = 0; i < 6; i++)
            {
                if (party[i] != null)
                {
                    if (pAct(party[i]))
                    {
                        buttons[i].IsEnabled = true;
                    }
                    else
                    {
                        buttons[i].IsEnabled = false;
                    }

                }
            }
        }

        private void switchScreen(bool forced)
        {
            SwitchCanvas.Visibility = Visibility.Visible;
            BattleCanvas.Visibility = Visibility.Collapsed;
            Button[] switchArray = new Button[] { s1, s2, s3, s4, s5, s6 };
            int j = 0;
            for (int i = 0; i < party.Length; ++i)
            {
                if (party[i] != null)
                {
                    if (usingItem)
                    {
                        switchArray[i].Content = party[i].Name;
                        switchArray[i].IsEnabled = true;
                    }
                    else
                    {
                        switchArray[i].Content = party[i].Name;

                        if (party[i].CurrentHP == 0)
                        {
                            switchArray[i].IsEnabled = false;
                        }
                        else
                        {
                            switchArray[i].IsEnabled = true;
                        }
                    }

                }
                else
                {
                    switchArray[i].IsEnabled = false;
                    switchArray[i].Content = "";
                }
            }

            if (forced)
            {
                back.IsEnabled = false;
            }
            else
            {
                back.IsEnabled = true;
            }

        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            if (inBattle)
            {
                BattleCanvas.Visibility = Visibility.Visible;
                SwitchCanvas.Visibility = Visibility.Collapsed;
            }
            else
            {
                SwitchCanvas.Visibility = Visibility.Collapsed;
            }

        }

        private void useOn(int i)
        {
            if (currentBagOpen == ItemType.HealAndStatus)
            {
                if (currentItem.GetType() == typeof(EvolutionItem))
                {
                    if (party[i].needToEvolve(currentItem.Name))
                    {
                        evolveScreen(party[i]);
                    }
                }
                else
                {
                    party[i].heal((currentItem as HealAndStatusItem).use());
                }
            }

            myBag.Remove(itemBox.SelectedItem as Item, currentBagOpen);
            usingItem = false;

            BagCanvas.Visibility = Visibility.Collapsed;
            SwitchCanvas.Visibility = Visibility.Collapsed;
            itemBox.Items.Clear();
        }

        public bool AttemptCapture(Pokeball b, Pokemon w)
        {
            itemBox.Items.Clear();

            if (w.ThrowBall(b))
            {
                w.MovesOnLevel = Pokebuilder.Instance.MovesOnLevelForPokemon(w.Number);
                int s = nextSpace();
                if (s == 6)
                {
                    pc.Add(w);
                }
                else
                {
                    party[s] = w;
                }

                battleController.finalMoveFinished = true;
                battleEnd("The Pokemon was caught!");
                return true;
            }
            else
            {
                return false;
            }



        }

        private void battleMoveSelected(BattleAction b)
        {
            bBack.Visibility = Visibility.Collapsed;

            Action battleEndEffects = (() =>
            {
                if (inBattle)
                {
                    endOfAttackPhaseEffects();
                }
            });

            MoveDictionary foeMove = FoesNextAttack();
            int myMovePriorty = MoveStore.Instance.get(b.md).Priority;

            BattleCanvas.Visibility = Visibility.Visible;
            battleController.finalMoveFinished = false;
            if (!b.isAttacking())
            {
                battleController.battleQueue.Enqueue(() => { textBlockBat.Text = FoeAttack(); });
                battleController.battleQueue.Enqueue(battleEndEffects);

                b1.IsEnabled = false;
                b2.IsEnabled = false;
                b3.IsEnabled = false;
                b4.IsEnabled = false;

                battleController.finalMoveFinished = false;
                textBlockBat.Text = b.Act();
            }
            else
            {
                if (foeMove.Priority == myMovePriorty)
                {
                    if (battleController.CurrentFoe.Speed > battleController.CurrentInBattle.Speed)
                    {
                        battleController.battleQueue.Enqueue(() => { textBlockBat.Text = b.Act(); });
                        battleController.battleQueue.Enqueue(battleEndEffects);
                        textBlockBat.Text = FoeAttack();

                    }
                    else
                    {

                        battleController.battleQueue.Enqueue(() => { textBlockBat.Text = FoeAttack(); });
                        battleController.battleQueue.Enqueue(battleEndEffects);
                        textBlockBat.Text = b.Act();

                    }
                }
                else
                {
                    if (foeMove.Priority > myMovePriorty)
                    {
                        battleController.battleQueue.Enqueue(() => { textBlockBat.Text = b.Act(); });
                        battleController.battleQueue.Enqueue(battleEndEffects);
                        textBlockBat.Text = FoeAttack();
                    }
                    else
                    {
                        battleController.battleQueue.Enqueue(() => { textBlockBat.Text = FoeAttack(); });
                        battleController.battleQueue.Enqueue(battleEndEffects);
                        textBlockBat.Text = b.Act();
                    }
                }



                b1.IsEnabled = false;
                b2.IsEnabled = false;
                b3.IsEnabled = false;
                b4.IsEnabled = false;


            }


        }

        private void endOfAttackPhaseEffects()
        {
            battleController.CurrentFoe.Protection = null;
            battleController.CurrentInBattle.Protection = null;
            //weather like hail and sandstorm
            battleController.battleQueue.Enqueue(() => { textBlockBat.Text = battleController.EndOfAttackPhaseEffects(); dealWithDamage(); });
            string ret = "";

            if (battleController.CurrentFoe.Leeching)
            {
                ret = battleController.CurrentFoe.EndOfTurnEffects((int)(Math.Ceiling(0.3 * battleController.CurrentInBattle.CurrentHP)));
                if (ret != "")
                {
                    battleController.battleQueue.Enqueue(() =>
                    {
                        textBlockBat.Text = battleController.CurrentFoe.EndOfTurnEffects((int)(Math.Ceiling(0.3 * battleController.CurrentInBattle.CurrentHP)));
                        dealWithDamage();
                    }
                    );
                }

            }
            else
            {
                ret = battleController.CurrentFoe.EndOfTurnEffects(0);
                if (ret != "")
                {
                    battleController.battleQueue.Enqueue(() =>
                    {
                        textBlockBat.Text = battleController.CurrentFoe.EndOfTurnEffects(0);
                        dealWithDamage();
                    }
                    );
                }

            }

            if (battleController.CurrentInBattle.Leeching)
            {
                ret = battleController.CurrentInBattle.EndOfTurnEffects((int)(Math.Ceiling(0.3 * battleController.CurrentFoe.CurrentHP)));
                if (ret != "")
                {
                    battleController.battleQueue.Enqueue(() => {
                        textBlockBat.Text = battleController.CurrentInBattle.EndOfTurnEffects((int)(Math.Ceiling(0.3 * battleController.CurrentFoe.CurrentHP)));
                        dealWithDamage();
                    }
                    );
                }

            }
            else
            {
                ret = battleController.CurrentInBattle.EndOfTurnEffects(0);
                if (ret != "")
                {
                    battleController.battleQueue.Enqueue(() => {
                        textBlockBat.Text = battleController.CurrentInBattle.EndOfTurnEffects(0);
                        dealWithDamage();
                    }
                    );
                }

            }

            if (battleController.battleQueue.Count > 0)
            {
                battleController.battleQueue.Dequeue().Invoke();
            }
            else
            {
                dealWithDamage();
            }



        }

        public void showPokeInfo(Pokemon p)
        {
            PokemonInfoSprite.Source = new BitmapImage(new Uri(("/images/front/" + p.Number + ".png"), UriKind.Relative));
            PokemonInfoScreen.Visibility = Visibility.Visible;
            PokeStatBox.Text = p.getStatsInfo();
            PokeMoveBox.Text = p.getMoveInfo();

            double o = Math.Pow(p.level, 3);

            InfoExp.Width = (Math.Max((double)p.Experience - o, 0) / (Math.Pow(p.level + 1, 3) - o)) * 160;
        }

        private void switchButtonClicked(int i)
        {
            if (usingItem)
            {
                if (inBattle)
                {
                    battleController.notAttacking = true;
                    battleMoveSelected(new BattleAction(useOn, i, party[i].Name + " was healed!"));
                }
                else
                {
                    useOn(i);
                }
            }
            else
            {
                if (inBattle)
                {
                    battleController.notAttacking = true;
                    battleMoveSelected(new BattleAction(switchOut, i, "Go " + party[i].Name));
                }
                else
                {
                    currentLookingPartyPokemon = i;
                    showPokeInfo(party[i]);
                }
            }
        }

        private void s1_Click(object sender, RoutedEventArgs e)
        {
            switchButtonClicked(0);
        }

        private void s5_Click(object sender, RoutedEventArgs e)
        {
            switchButtonClicked(4);
        }

        private void s4_Click(object sender, RoutedEventArgs e)
        {
            switchButtonClicked(3);
        }

        private void s3_Click(object sender, RoutedEventArgs e)
        {
            switchButtonClicked(2);
        }

        private void s2_Click(object sender, RoutedEventArgs e)
        {
            switchButtonClicked(1);
        }

        private void s6_Click(object sender, RoutedEventArgs e)
        {
            switchButtonClicked(5);
        }

        public void userSwitchOutEffects()
        {
            if (battleController.CurrentInBattle.Status == StatusType.Confusion || battleController.CurrentInBattle.Status == StatusType.Infatuation || battleController.CurrentInBattle.Status == StatusType.NoHoldItem || battleController.CurrentInBattle.Status == StatusType.HealBlock || battleController.CurrentInBattle.Status == StatusType.NoImmunity || battleController.CurrentInBattle.Status == StatusType.LeechSeed || battleController.CurrentInBattle.Status == StatusType.PerishSong || battleController.CurrentInBattle.Status == StatusType.Telekenesis || battleController.CurrentInBattle.Status == StatusType.Torment || battleController.CurrentInBattle.Status == StatusType.NightMare)
            {
                battleController.CurrentInBattle.Status = StatusType.Null;
            }
        }

        private void switchOut(int i)
        {
            userSwitchOutEffects();
            battleController.CurrentInBattle = party[i];
            party[i] = party[0];
            party[0] = battleController.CurrentInBattle;

            if (inBattle)
            {
                MySprite.Source = new BitmapImage(new Uri(("/images/back/" + party[0].Number + ".png"), UriKind.Relative));
                textBlockBat.Text = battleController.InvokeMyEntryEffects();
                BattleCanvas.Visibility = Visibility.Visible;
                SwitchCanvas.Visibility = Visibility.Collapsed;
                battleController.pokesThatBattled.Add(battleController.CurrentInBattle);
                dealWithDamage();

            }

        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentInteraction != null)
            {
                currentInteraction.Interact();
            }

            if (!moretoDo)
            {
                closeYesNo();
            }

        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            ((NPCInteraction)currentInteraction).NoInteract();
            if (currentInteraction.hasNoOption)
            {
                if (!moretoDo)
                {
                    closeYesNo();
                }
            }
            else
            {
                closeYesNo();
            }
        }

        private void pokemonButton_Click(object sender, RoutedEventArgs e)
        {
            switchScreen(false);
        }

        private void bagButton_Click(object sender, RoutedEventArgs e)
        {
            currentBagOpen = ItemType.HealAndStatus;
            BagCanvas.Visibility = Visibility.Visible;
            useButton.Content = "Use";
            MoneyInBagTextBlock.Text = "Money: " + money;
            BagSellQUp.Visibility = Visibility.Collapsed;
            BagSellQDown.Visibility = Visibility.Collapsed;
            KeyItemsButton.IsEnabled = true;
            StartCanvas.Visibility = Visibility.Collapsed;
            ItemsButton_Click(null, null);
        }

        private void closeBagButton_Click(object sender, RoutedEventArgs e)
        {
            itemBox.Items.Clear();
            BagCanvas.Visibility = Visibility.Collapsed;
            if (!inBattle)
            {
                RouteCanvas.Visibility = Visibility.Visible;
            }
        }

        private void ItemsButton_Click(object sender, RoutedEventArgs e)
        {
            currentBagOpen = ItemType.HealAndStatus;
            itemBox.Items.Clear();
            foreach (HealAndStatusItem i in myBag.items.Keys)
            {
                itemBox.Items.Add(i);
            }
        }

        private void BallsButton_Click(object sender, RoutedEventArgs e)
        {
            currentBagOpen = ItemType.Pokeball;
            itemBox.Items.Clear();
            foreach (Pokeball i in myBag.balls.Keys)
            {
                itemBox.Items.Add(i);
            }
        }

        private void KeyItemsButton_Click(object sender, RoutedEventArgs e)
        {
            currentBagOpen = ItemType.KeyItem;
            itemBox.Items.Clear();
            foreach (KeyItem i in myBag.keyItems.Keys)
            {
                itemBox.Items.Add(i);
            }
        }

        private void itemBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (itemBox.SelectedItem != null)
            {
                itemInfoBox.Text = (itemBox.SelectedItem as Item).Description;

                if (BagSellQUp.Visibility == Visibility.Collapsed)
                {
                    if (currentBagOpen == ItemType.HealAndStatus)
                    {
                        quantityBox.Text = "Quantity in bag: " + myBag.items[itemBox.SelectedItem as HealAndStatusItem];
                    }
                    else if (currentBagOpen == ItemType.KeyItem)
                    {
                        quantityBox.Text = "Quantity in bag: " + myBag.keyItems[itemBox.SelectedItem as KeyItem];
                    }
                    else if (currentBagOpen == ItemType.Pokeball)
                    {
                        quantityBox.Text = "Quantity in bag: " + myBag.balls[itemBox.SelectedItem as Pokeball];
                    }
                }
                else
                {
                    quantityBox.Text = "Quantity: 1";
                }
            }
            else
            {
                itemInfoBox.Text = "";
                quantityBox.Text = "Quantity in bag: ";
            }
        }

        private void LearnNewMove(Pokemon p, string m)
        {
            if (p.Moves.Where((mov) => mov == null).Any())
            {
                battleController.battleQueue.Enqueue(() => textBlockBat.Text = p.Name + "learnt the move " + m);
                p.Moves[p.getNextAvailableMoveSlot()] = m;
            }
            else
            {
                battleController.battleQueue.Enqueue(() => textBlockBat.Text = p.Name + " is trying to learn the move " + m);
                battleController.battleQueue.Enqueue(() => openYesNo("Would you like to delete a move in order to learn " + m + "?", true));
                currentInteraction = new UxInteraction(() =>
                {
                    deletingAMove = true;
                    deletingAMoveFrom = p;
                    replacingMove = m;
                    string[] str = battleController.getButtonContent();
                    b1.Content = str[0];
                    b2.Content = str[1];
                    b3.Content = str[2];
                    b4.Content = str[3];
                    b1.IsEnabled = true;
                    b2.IsEnabled = true;
                    b3.IsEnabled = true;
                    b4.IsEnabled = true;
                });
            }
        }

        private void checkAlive()
        {
            if (battleController.CurrentFoe.CurrentHP == 0)
            {
                battleController.battleQueue.Clear();
                battleController.battleQueue.Enqueue(() => textBlockBat.Text = "The foe has fainted!");

                List<String> comments = battleController.calculateExperience();
                int i = 1;
                battleController.removeFromFoe(battleController.CurrentFoe);

                battleController.CurrentInBattle.foeSwitchOutResetStatus();
                int p = 0;

                //process comments on exp gain/level up/new move
                foreach (String s in comments)
                {
                    if (s.Contains("NEW-MOVE"))
                    {
                        Guid id = Guid.Parse(s.Split(':')[1].Split(',')[0]);
                        string moveName = s.Split(':')[1].Split(',')[1];
                        LearnNewMove(party.Where((poke) => poke.ID == id).First(), moveName);
                    }
                    else
                    {
                        battleController.battleQueue.Enqueue(() => textBlockBat.Text = s);
                    }
                }

                if (battleController.battleEnded())
                {
                    b1.IsEnabled = false;
                    b2.IsEnabled = false;
                    b3.IsEnabled = false;
                    b4.IsEnabled = false;
                    battleEnd("The foe was defeated!");
                }
                else
                {
                    battleController.finalMoveFinished = true;
                    battleController.battleQueue.Enqueue(() =>
                    {
                        textBlockBat.Text = "The foe has sent out " + battleController.CurrentFoe.Name;
                        FoeNameLabel.Content = battleController.CurrentFoe.Name + " Lvl:" + battleController.CurrentFoe.level;
                        dealWithDamage();
                        PokesSeenID.Add(battleController.CurrentFoe.Number);
                        battleController.battleQueue.Enqueue(() => { textBlockBat.Text = battleController.InvokeFoeEntryEffects(); dealWithDamage(); });
                    });

                }
            }

            if (battleController.CurrentInBattle.CurrentHP == 0)
            {
                battleController.battleQueue.Clear();
                battleController.finalMoveFinished = true;
                if (moreAlive())
                {
                    battleController.battleQueue.Enqueue(() => textBlockBat.Text = battleController.CurrentInBattle.Name + " has fainted");
                    battleController.battleQueue.Enqueue(() => switchScreen(true));
                }
                else
                {
                    lost = true;
                    battleEnd("You have lost the battle! Player whited out!");
                }
            }
        }

        private void useButton_Click(object sender, RoutedEventArgs e)
        {
            if (itemBox.SelectedItem != null)
            {
                usingItem = true;
                currentItem = itemBox.SelectedItem as Item;
            }

            if (BagSellQUp.Visibility == Visibility.Visible)
            {
                //selling
                int amount = int.Parse(quantityBox.Text.Split(' ')[1]);
                myBag.Remove(currentItem, currentBagOpen, amount);
                money += (int)((double)currentItem.Price * (double)amount * 0.8);
                itemInfoBox.Text = "Sold!";
                MoneyInBagTextBlock.Text = "Money: " + money.ToString();
            }
            else
            {
                if (currentBagOpen == ItemType.HealAndStatus)
                {
                    if (currentItem.GetType() == typeof(EvolutionItem))
                    {
                        switchScreen((p) => { return (currentItem as EvolutionItem).CanEvolve(p.Number); });
                    }
                    else
                    {
                        switchScreen(false);
                    }

                }
                else if (currentBagOpen == ItemType.Pokeball)
                {
                    if (inBattle)
                    {
                        //but wild only
                        battleController.notAttacking = true;
                        BagCanvas.Visibility = Visibility.Collapsed;
                        SwitchCanvas.Visibility = Visibility.Collapsed;
                        myBag.Remove(currentItem, ItemType.Pokeball);
                        battleMoveSelected(new BattleAction(AttemptCapture, currentItem as Pokeball, battleController.CurrentFoe, ""));
                    }

                }
            }



        }

        private void BtnWithdraw_Click(object sender, RoutedEventArgs e)
        {
            if (partyPokemonLst.Items.Count < 6 && pcPokemoLstn.SelectedItem != null)
            {
                partyPokemonLst.Items.Add(pcPokemoLstn.SelectedItem);
                pcPokemoLstn.Items.Remove(pcPokemoLstn.SelectedItem);
            }
        }

        private void BtnDeposit_Click(object sender, RoutedEventArgs e)
        {
            if (partyPokemonLst.Items.Count > 1 && partyPokemonLst.SelectedItem != null)
            {
                pcPokemoLstn.Items.Add(partyPokemonLst.SelectedItem);
                partyPokemonLst.Items.Remove(partyPokemonLst.SelectedItem);
            }

        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            var doc = new XDocument();

            try
            {
                using (IsolatedStorageFile isoFile = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream("save.xml", FileMode.Open, isoFile))
                    {
                        using (StreamReader sw = new StreamReader(isoStream))
                        {
                            doc = XDocument.Load(sw);
                        }
                    }
                }

                NPCInteraction areYouSureSave = new NPCInteraction("A save file already exists, overwrite it?", () => save(doc));
                currentInteraction = areYouSureSave;
                openYesNo("A save file already exists, overwrite it?", false);

            }
            catch (Exception)
            {
                //no save file
                save(doc);
            }
        }

        private void openSaveFile()
        {
            XDocument doc = new XDocument();
            using (IsolatedStorageFile isoFile = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream("save.xml", FileMode.Open, isoFile))
                {
                    using (StreamReader sw = new StreamReader(isoStream))
                    {
                        doc = XDocument.Load(sw);
                    }
                }
            }

            var elem = doc.Element("Save");

            currentRoute = RouteStore.Instance.Get(elem.Element("route").Value);
            playerTop = Int32.Parse(elem.Element("playerTop").Value);
            playerLeft = Int32.Parse(elem.Element("playerLeft").Value);
            trainerName = elem.Element("playerName").Value;

            if (elem.Element("playerGender").Value.Equals("boy"))
            {
                trainerGender = gender.boy;
            }
            else
            {
                trainerGender = gender.girl;
            }

            var p = Pokebuilder.Instance.openPokes(elem.Element("party")).ToArray();
            for (int i = 0; i < 6; ++i)
            {
                try
                {
                    party[i] = p[i];
                }
                catch
                {
                    break;
                }
            }

            pc = Pokebuilder.Instance.openPokes(elem.Element("pc"));

            var seenPokes = elem.Element("PokemonSeen");
            foreach (var v in seenPokes.Elements())
            {
                PokesSeenID.Add(int.Parse(v.Value));
            }


            var ints = elem.Element("Interactions");
            profOak.currentInter = int.Parse(ints.Element("oak").Value);
            trainer.currentInter = int.Parse(ints.Element("trainer").Value);
            gymTurn1Trainer.currentInter = int.Parse(ints.Element("gymTurn1").Value);
            gymLeaderTurn.currentInter = int.Parse(ints.Element("gymTurnLeader").Value);



        }
        private bool save(XDocument doc)
        {
            doc = new XDocument();
            var saveFile = new XElement("Save");
            var route = new XElement("route", RouteStore.Instance.Get(currentRoute));
            var pTop = new XElement("playerTop", playerTop);
            var pLeft = new XElement("playerLeft", playerLeft);
            var playerName = new XElement("playerName", trainerName);
            var playerGender = new XElement("playerGender", trainerGender);

            saveFile.Add(route);
            saveFile.Add(Pokebuilder.Instance.savePokes(party, "party"));
            saveFile.Add(Pokebuilder.Instance.savePokes(pc.ToArray(), "pc"));
            saveFile.Add(pTop);
            saveFile.Add(pLeft);
            saveFile.Add(playerName);
            saveFile.Add(playerGender);

            var PokemonSeen = new XElement("PokemonSeen");
            foreach (var i in PokesSeenID)
            {
                PokemonSeen.Add(new XElement("Number", i));
            }
            saveFile.Add(PokemonSeen);

            var InteractionsSave = new XElement("Interactions");
            InteractionsSave.Add(new XElement("oak", profOak.currentInter));
            InteractionsSave.Add(new XElement("trainer", trainer.currentInter));
            InteractionsSave.Add(new XElement("gymTurn1", gymTurn1Trainer.currentInter));
            InteractionsSave.Add(new XElement("gymTurnLeader", gymLeaderTurn.currentInter));

            saveFile.Add(InteractionsSave);
            doc.Add(saveFile);

            using (IsolatedStorageFile isoFile = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream("save.xml", FileMode.Create, isoFile))
                {
                    using (StreamWriter sw = new StreamWriter(isoStream))
                    {
                        doc.Save(sw);
                    }
                }
            }

            return true;
        }

        private void ContinueButton_Click(object sender, RoutedEventArgs e)
        {
            openSaveFile();
            SaveAndPpenCanvas.Visibility = Visibility.Collapsed;
            RouteCanvas.Visibility = Visibility.Visible;
            cleanAndDraw(currentRoute);
        }

        private void NewGameButton_Click(object sender, RoutedEventArgs e)
        {
            SaveAndPpenCanvas.Visibility = Visibility.Collapsed;
            initCanvas.Visibility = Visibility.Visible;
        }

        private void sendToFrontButton_Click(object sender, RoutedEventArgs e)
        {
            switchOut(currentLookingPartyPokemon);
            switchScreen(false);
        }

        private void closePokeInfo_Click(object sender, RoutedEventArgs e)
        {
            PokemonInfoScreen.Visibility = Visibility.Collapsed;
        }

        private void quantUp_Click(object sender, RoutedEventArgs e)
        {
            int quantity = int.Parse(quantityBoxBuying.Text.Split(' ')[1]);
            quantityBoxBuying.Text = "Quantity: " + ++quantity;
        }

        private void QuantDown_Click(object sender, RoutedEventArgs e)
        {
            int quantity = int.Parse(quantityBoxBuying.Text.Split(' ')[1]);
            if (quantity > 1)
            {
                quantityBoxBuying.Text = "Quantity: " + --quantity;
            }
        }

        private void buyButton_Click(object sender, RoutedEventArgs e)
        {
            if (buyingItem != null && itemBoxBuying.SelectedItem != null)
            {
                int quantity = int.Parse(quantityBoxBuying.Text.Split(' ')[1]);
                int amount = quantity * buyingItem.Price;

                if (amount <= money)
                {
                    myBag.Add(buyingItem, quantity);
                    money -= amount;

                    itemInfoBoxBuying.Text = "Purchased!";
                    moneyTextBox.Text = "Money: " + money;
                }
            }
            else
            {
                itemInfoBox.Text = "Not enough money!";
            }

        }

        private void closeShopButton_Click(object sender, RoutedEventArgs e)
        {
            BuyingCanvas.Visibility = Visibility.Collapsed;
            RouteCanvas.Visibility = Visibility.Visible;
        }

        private void itemBoxBuying_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (itemBoxBuying.SelectedItem != null)
            {
                buyingItem = itemBoxBuying.SelectedItem as Item;
                itemInfoBoxBuying.Text = buyingItem.Description;
                quantityBoxBuying.Text = "Quantity: 1";
                PriceBoxBuying.Text = "Price: " + buyingItem.Price;
            }
            else
            {
                itemInfoBoxBuying.Text = "";
                quantityBoxBuying.Text = "Quantity: 0";
                PriceBoxBuying.Text = "Price: 0";
            }
        }

        private void CloseYesNoButton_Click(object sender, RoutedEventArgs e)
        {
            CloseYesNoButton.Visibility = Visibility.Collapsed;
            YesNoCanvas.Visibility = Visibility.Collapsed;
        }

        private void BagSellQUp_Click(object sender, RoutedEventArgs e)
        {
            if (itemBox.SelectedItem != null)
            {
                int quantity = int.Parse(quantityBox.Text.Split(' ')[1]);
                if (quantity < myBag.getCountForItem(itemBox.SelectedItem as Item, currentBagOpen))
                {
                    quantityBox.Text = "Quantity: " + ++quantity;
                }
            }
        }

        private void BagSellQDown_Click(object sender, RoutedEventArgs e)
        {
            if (itemBox.SelectedItem != null)
            {
                int quantity = int.Parse(quantityBox.Text.Split(' ')[1]);
                if (quantity > 1)
                {
                    quantityBox.Text = "Quantity: " + --quantity;
                }
            }
        }

        private void trainerButton_Click(object sender, RoutedEventArgs e)
        {
            RouteCanvas.Visibility = Visibility.Collapsed;
            StartCanvas.Visibility = Visibility.Collapsed;
            TrainerCardCanvas.Visibility = Visibility.Visible;

            int i = 0;
            for (i = 0; i < 5; i++)
            {
                try
                {
                    bool b = party[i].ID != Guid.Empty;
                }
                catch
                {
                    break;
                }
            }
            i += pc.Count;
            CardCaughtAmount.Text = "Caught: " + i + " / 151";
            CardMoneyText.Text = "Money: £" + money;
            CardNameText.Text = "Name: " + trainerName;

            BadgeOne.Visibility = boolToVis(badge1);
            BadgeTwo.Visibility = boolToVis(badge2);
            BadgeThree.Visibility = boolToVis(badge3);
            BadgeFour.Visibility = boolToVis(badge4);
            BadgeFive.Visibility = boolToVis(badge5);
            BadgeSix.Visibility = boolToVis(badge6);
            BadgeSeven.Visibility = boolToVis(badge7);
            BadgeEight.Visibility = boolToVis(badge8);
        }

        private Visibility boolToVis(bool b)
        {
            if (b)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        public void openYesNo(string t, bool f)
        {
            openYesNo(t, "Yes", "No", f, false);
        }

        private void openYesNo(string text, string y, string n, bool forced, bool close)
        {
            YesNoCanvas.Visibility = Visibility.Visible;
            YesNoText.Text = text;
            YesButton.Content = y;
            NoButton.Content = n;

            if (forced)
            {
                NoButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                NoButton.Visibility = Visibility.Visible;
            }

            if (close)
            {
                CloseYesNoButton.Visibility = Visibility.Visible;
            }
            else
            {
                CloseYesNoButton.Visibility = Visibility.Collapsed;
            }

        }

        private void closeYesNo()
        {
            try
            {
                if ((currentInteraction as NPCInteraction).nextInter == null)
                {
                    currentInteraction = null;
                }
            }
            catch
            {
                currentInteraction = null;
            }

            YesNoCanvas.Visibility = Visibility.Collapsed;
        }

        private void bBack_Click(object sender, RoutedEventArgs e)
        {
            fightButtonPressed = false;
            b1.Content = "Fight";
            b2.Content = "Bag";
            b3.Content = "Pokemon";
            b4.Content = "Run";
            b4.IsEnabled = battleController.isWildBattle;
        }

        private void readyToStartButton_Click(object sender, RoutedEventArgs e)
        {
            //do some checking
            if (BoyRadbutton.IsChecked == true)
            {
                trainerGender = gender.boy;
            }
            else
            {
                trainerGender = gender.girl;
            }

            trainerName = TrainerNameTextBox.Text;

            initCanvas.Visibility = Visibility.Collapsed;
            RouteCanvas.Visibility = Visibility.Visible;
        }

        private void TrainerCardClose_Click(object sender, RoutedEventArgs e)
        {
            RouteCanvas.Visibility = Visibility.Visible;
            StartCanvas.Visibility = Visibility.Visible;
            TrainerCardCanvas.Visibility = Visibility.Collapsed;
        }

        private void PokedexClose_Click(object sender, RoutedEventArgs e)
        {
            RouteCanvas.Visibility = Visibility.Visible;
            StartCanvas.Visibility = Visibility.Visible;
            PokedexSelectCanvas.Visibility = Visibility.Collapsed;
        }

        private void PokedexClose_Click_1(object sender, RoutedEventArgs e)
        {
            PokedexSelectCanvas.Visibility = Visibility.Visible;
            PokedexCanvas.Visibility = Visibility.Collapsed;
        }

        private void PokedexOpenButton_Click(object sender, RoutedEventArgs e)
        {
            if (PokedexSelectListBox.SelectedItem != null)
            {
                PokedexEntryNameText.Text = (PokedexSelectListBox.SelectedItem as Pokemon).Name;
                if (PokedexEntryNameText.Text == "???")
                {
                    PokedexEntrySprite.Source = null;
                    PokedexFlavourText.Text = "";
                }
                else
                {
                    PokedexEntrySprite.Source = new BitmapImage(new Uri(("/images/front/" + (PokedexSelectListBox.SelectedItem as Pokemon).Number + ".png"), UriKind.Relative));
                    if ((PokedexSelectListBox.SelectedItem as Pokemon).Confused)
                    {
                        PokedexFlavourText.Text = "???";
                    }
                    else
                    {
                        PokedexFlavourText.Text = Pokebuilder.Instance.getFlavourText((PokedexSelectListBox.SelectedItem as Pokemon).Number);
                    }
                }
            }
            PokedexSelectCanvas.Visibility = Visibility.Collapsed;
            PokedexCanvas.Visibility = Visibility.Visible;
        }

        private void pokedexButton_Click(object sender, RoutedEventArgs e)
        {
            PokedexSelectCanvas.Visibility = Visibility.Visible;
            RouteCanvas.Visibility = Visibility.Collapsed;
            StartCanvas.Visibility = Visibility.Collapsed;

            addRowsToPokedexGrid();
        }

        private void addRowsToPokedexGrid()
        {
            foreach (var v in Pokebuilder.Instance.getAllPokesIDName())
            {
                var t = party.Where((p) => { if (p == null) { return false; } else { return p.Number == v.Number; } }).Any();
                var y = pc.Where((p) => { return p.Number == v.Number; }).Any();

                if (t || y)
                {
                    PokedexSelectListBox.Items.Add(v);
                }
                else
                {
                    if (PokesSeenID.Contains(v.Number))
                    {
                        PokedexSelectListBox.Items.Add(new Pokemon() { Number = v.Number, Name = v.Name, Confused = true });
                    }
                    else
                    {
                        PokedexSelectListBox.Items.Add(new Pokemon() { Number = v.Number, Name = "???" });
                    }
                }
            }
        }

        private void BtnClosePC_Click(object sender, RoutedEventArgs e)
        {
            pc.Clear();
            foreach (Pokemon p in pcPokemoLstn.Items)
            {
                pc.Add(p);
            }

            for (int i = 0; i < 6; i++)
            {
                party[i] = null;
            }

            for (int i = 0; i < partyPokemonLst.Items.Count(); i++)
            {

                party[i] = (Pokemon)partyPokemonLst.Items[i];

            }

            pcScreenCanvas.Visibility = Visibility.Collapsed;
            partyPokemonLst.Items.Clear();
            pcPokemoLstn.Items.Clear();
        }
    }
}




