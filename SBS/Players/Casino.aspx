<%@ Page Language="VB" MasterPageFile="~/SBS/Players/Player.master" AutoEventWireup="false"
    CodeFile="Casino.aspx.vb" Inherits="SBS_Players_Casino" %>


<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="Server">
    <script type="text/javascript">
        function OnGameClicked() {

            try {

                $('#<%=lbnCasino.ClientID %>').click();
   } catch (e) {

   }

}
    </script>
    <div class="panel panel-grey">
        <div class="panel-heading">ALL GAMES</div>
        <div class="panel-body">
            <asp:Button ID="lbnCasino" runat="server" Style="display: none" Text="btn btn-primary" />

            <table class="casinoTab" width="100%">
                <tbody>
                    <tr>
                        <td class="casinoTab" colspan="7" align="center" valign="top">
                            <div class="casinoTab">
                                <center>
                            <div class="tabdesc">
                            </div>
                            <table class="casinogames">
                                <tbody>
                                    <tr>
                                        <td class="casinogames">
                                            <table class="casinogame" cellspacing="0" cellpadding="0">
                                                <tbody>
                                                    <tr valign="bottom">
                                                        <td valign="top" class="gameiconBrown">
                                                            <a href="#" onclick="OnGameClicked();">
                                                                <img border="0" class="lobbyimg" alt="Blackjack" src="/images/Blackjack.jpg"></a>
                                                        </td>
                                                    </tr>
                                                    <tr valign="middle" height="41">
                                                        <td class="gametextBrown">
                                                            Multihand Blackjack<br>
                                                            6 deck<br>
                                                            Min. Bet: $1, Max. Bet: $50
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </td>
                                        <td class="casinogames">
                                            <table class="casinogame" cellspacing="0" cellpadding="0">
                                                <tbody>
                                                    <tr valign="bottom">
                                                        <td valign="top" class="gameiconBrown">
                                                            <a href="#" onclick="OnGameClicked();">
                                                                <img border="0" class="lobbyimg" alt="Blackjack" src="/images/Blackjack.jpg"></a>
                                                        </td>
                                                    </tr>
                                                    <tr valign="middle" height="41">
                                                        <td class="gametextBrown">
                                                            Blackjack<br>
                                                            6 deck<br>
                                                            Min. Bet: $5, Max. Bet: $100
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </td>
                                        <td class="casinogames">
                                            <table class="casinogame" cellspacing="0" cellpadding="0">
                                                <tbody>
                                                    <tr valign="bottom">
                                                        <td valign="top" class="gameiconBrown">
                                                            <a href="#" onclick="OnGameClicked();">
                                                                <img border="0" class="lobbyimg" alt="Blackjack Switch" src="/images/BJSwitch.jpg"></a>
                                                        </td>
                                                    </tr>
                                                    <tr valign="middle" height="41">
                                                        <td class="gametextBrown">
                                                            Blackjack Switch<br>
                                                            Switch cards between 2 hands!<br>
                                                            Min. Bet: $1, Max. Bet: $50
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="casinogames">
                                            <table class="casinogame" cellspacing="0" cellpadding="0">
                                                <tbody>
                                                    <tr valign="bottom">
                                                        <td valign="top" class="gameiconBrown">
                                                            <a href="#" onclick="OnGameClicked();">
                                                                <img border="0" class="lobbyimg" alt="Blackjack Double Exposure" src="/images/BJDoubleExposure.jpg"></a>
                                                        </td>
                                                    </tr>
                                                    <tr valign="middle" height="41">
                                                        <td class="gametextBrown">
                                                            Blackjack Double Exposure<br>
                                                            See both dealer cards!<br>
                                                            Min. Bet: $1, Max. Bet: $50
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </td>
                                        <td class="casinogames">
                                            <table class="casinogame" cellspacing="0" cellpadding="0">
                                                <tbody>
                                                    <tr valign="bottom">
                                                        <td valign="top" class="gameiconBrown">
                                                            <a href="#" onclick="OnGameClicked();">
                                                                <img border="0" class="lobbyimg" alt="Spanish Blackjack" src="/images/BJSpanish21.jpg"></a>
                                                        </td>
                                                    </tr>
                                                    <tr valign="middle" height="41">
                                                        <td class="gametextBrown">
                                                            Spanish Blackjack<br>
                                                            Min. Bet: $1, Max. Bet: $50
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </td>
                                        <td class="casinogames">
                                            <table class="casinogame" cellspacing="0" cellpadding="0">
                                                <tbody>
                                                    <tr valign="bottom">
                                                        <td valign="top" class="gameiconGreen">
                                                            <a href="#" onclick="OnGameClicked();">
                                                                <img border="0" class="lobbyimg" alt="Roulette" src="/images/Roulette-American.jpg"></a>
                                                        </td>
                                                    </tr>
                                                    <tr valign="middle" height="41">
                                                        <td class="gametextGreen">
                                                            Roulette<br>
                                                            Min. Bet: $1, Max. Bet: $50
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="casinogames">
                                            <table class="casinogame" cellspacing="0" cellpadding="0">
                                                <tbody>
                                                    <tr valign="bottom">
                                                        <td valign="top" class="gameiconGreen">
                                                            <a href="#" onclick="OnGameClicked();">
                                                                <img border="0" class="lobbyimg" alt="Craps" src="/images/Craps.jpg"></a>
                                                        </td>
                                                    </tr>
                                                    <tr valign="middle" height="41">
                                                        <td class="gametextGreen">
                                                            Craps<br>
                                                            Min. Bet: $1, Max. Bet: $100
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </td>
                                        <td class="casinogames">
                                            <table class="casinogame" cellspacing="0" cellpadding="0">
                                                <tbody>
                                                    <tr valign="bottom">
                                                        <td valign="top" class="gameiconGreen">
                                                            <a href="#" onclick="OnGameClicked();">
                                                                <img border="0" class="lobbyimg" alt="Baccarat" src="/images/Baccarat.jpg"></a>
                                                        </td>
                                                    </tr>
                                                    <tr valign="middle" height="41">
                                                        <td class="gametextGreen">
                                                            Baccarat<br>
                                                            Min. Bet: $1, Max. Bet: $50
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </td>
                                        <td class="casinogames">
                                            <table class="casinogame" cellspacing="0" cellpadding="0">
                                                <tbody>
                                                    <tr valign="bottom">
                                                        <td valign="top" class="gameiconPurple">
                                                            <a href="#" onclick="OnGameClicked();">
                                                                <img border="0" class="lobbyimg" alt="Caribbean Stud Poker" src="/images/Stud-Poker.jpg"></a>
                                                        </td>
                                                    </tr>
                                                    <tr valign="middle" height="41">
                                                        <td class="gametextPurple">
                                                            Stud Poker<br>
                                                            Min. Bet: $1, Max. Bet: $25
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="casinogames">
                                            <table class="casinogame" cellspacing="0" cellpadding="0">
                                                <tbody>
                                                    <tr valign="bottom">
                                                        <td valign="top" class="gameiconPurple">
                                                            <a href="#" onclick="OnGameClicked();">
                                                                <img border="0" class="lobbyimg" alt="Pai Gow Poker" src="/images/Pai-Gow-Poker.jpg"></a>
                                                        </td>
                                                    </tr>
                                                    <tr valign="middle" height="41">
                                                        <td class="gametextPurple">
                                                            <br>
                                                            Min. Bet: $1, Max. Bet: $50
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </td>
                                        <td class="casinogames">
                                            <table class="casinogame" cellspacing="0" cellpadding="0">
                                                <tbody>
                                                    <tr valign="bottom">
                                                        <td valign="top" class="gameiconPurple">
                                                            <a href="#" onclick="OnGameClicked();">
                                                                <img border="0" class="lobbyimg" alt="Let It Ride Poker" src="/images/LetitRide-Poker.jpg"></a>
                                                        </td>
                                                    </tr>
                                                    <tr valign="middle" height="41">
                                                        <td class="gametextPurple">
                                                            Let It Ride Poker<br>
                                                            Min. Bet: $1, Max. Bet: $25
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </td>
                                        <td class="casinogames">
                                            <table class="casinogame" cellspacing="0" cellpadding="0">
                                                <tbody>
                                                    <tr valign="bottom">
                                                        <td valign="top" class="gameiconPurple">
                                                            <a href="#" onclick="OnGameClicked();">
                                                                <img border="0" class="lobbyimg" alt="Three Card Poker" src="/images/ThreeCard-poker.jpg"></a>
                                                        </td>
                                                    </tr>
                                                    <tr valign="middle" height="41">
                                                        <td class="gametextPurple">
                                                            Dealer opens with single Q,<br>
                                                            otherwise Pays 1:1 ante &amp; play<br>
                                                            Min. Bet: $1, Max. Bet: $100
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="casinogames">
                                            <table class="casinogame" cellspacing="0" cellpadding="0">
                                                <tbody>
                                                    <tr valign="bottom">
                                                        <td valign="top" class="gameiconRed">
                                                            <a href="#" onclick="OnGameClicked();">
                                                                <img border="0" class="lobbyimg" alt="Video Poker - Jacks Or Better" src="/images/JacksorBetter-VideoPoker.jpg"></a>
                                                        </td>
                                                    </tr>
                                                    <tr valign="middle" height="41">
                                                        <td class="gametextRed">
                                                            Jacks or Better<br>
                                                            Min. Bet: 25¢, Max. Bet: $25
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </td>
                                        <td class="casinogames">
                                            <table class="casinogame" cellspacing="0" cellpadding="0">
                                                <tbody>
                                                    <tr valign="bottom">
                                                        <td valign="top" class="gameiconRed">
                                                            <a href="#" onclick="OnGameClicked();">
                                                                <img border="0" class="lobbyimg" alt="Video Poker - Deuces Wild" src="/images/DeucesWild-VideoPoker.jpg"></a>
                                                        </td>
                                                    </tr>
                                                    <tr valign="middle" height="41">
                                                        <td class="gametextRed">
                                                            Deuces Wild<br>
                                                            Min. Bet: 25¢, Max. Bet: $25
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </td>
                                        <td class="casinogames">
                                            <table class="casinogame" cellspacing="0" cellpadding="0">
                                                <tbody>
                                                    <tr valign="bottom">
                                                        <td valign="top" class="gameiconRed">
                                                            <a href="#" onclick="OnGameClicked();">
                                                                <img border="0" class="lobbyimg" alt="Video Poker - Jokers Wild" src="/images/Jokers-Wild-VideoPoker.jpg"></a>
                                                        </td>
                                                    </tr>
                                                    <tr valign="middle" height="41">
                                                        <td class="gametextRed">
                                                            Jokers Wild<br>
                                                            Min. Bet: 25¢, Max. Bet: $25
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="casinogames">
                                            <table class="casinogame" cellspacing="0" cellpadding="0">
                                                <tbody>
                                                    <tr valign="bottom">
                                                        <td valign="top" class="gameiconRed">
                                                            <a href="#" onclick="OnGameClicked();">
                                                                <img border="0" class="lobbyimg" alt="Video Poker - Aces &amp; Eights" src="/images/AcesAndEights-VideoPoker.jpg"></a>
                                                        </td>
                                                    </tr>
                                                    <tr valign="middle" height="41">
                                                        <td class="gametextRed">
                                                            Aces and Eights<br>
                                                            Min. Bet: 25¢, Max. Bet: $25
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </td>
                                        <td class="casinogames">
                                            <table class="casinogame" cellspacing="0" cellpadding="0">
                                                <tbody>
                                                    <tr valign="bottom">
                                                        <td valign="top" class="gameiconRed">
                                                            <a href="#" onclick="OnGameClicked();">
                                                                <img border="0" class="lobbyimg" alt="Video Poker - Double Bonus" src="/images/DoubleBonus-VideoPoker.jpg"></a>
                                                        </td>
                                                    </tr>
                                                    <tr valign="middle" height="41">
                                                        <td class="gametextRed">
                                                            Double Bonus<br>
                                                            Min. Bet: 25¢, Max. Bet: $5
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </td>
                                        <td class="casinogames">
                                            <table class="casinogame" cellspacing="0" cellpadding="0">
                                                <tbody>
                                                    <tr valign="bottom">
                                                        <td valign="top" class="gameiconRed">
                                                            <a href="#" onclick="OnGameClicked();">
                                                                <img border="0" class="lobbyimg" alt="Video Poker - Double Double Bonus" src="/images/DoubleDoubleBonus-VideoPoker.jpg"></a>
                                                        </td>
                                                    </tr>
                                                    <tr valign="middle" height="41">
                                                        <td class="gametextRed">
                                                            Double Double Bonus<br>
                                                            Min. Bet: 25¢, Max. Bet: $5
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="casinogames">
                                            <table class="casinogame" cellspacing="0" cellpadding="0">
                                                <tbody>
                                                    <tr valign="bottom">
                                                        <td valign="top" class="gameiconRed">
                                                            <a href="#" onclick="OnGameClicked();">
                                                                <img border="0" class="lobbyimg" alt="Video Poker - Jacks Or Better - 25 Line" src="/images/JacksorBetter25-VideoPoker.jpg"></a>
                                                        </td>
                                                    </tr>
                                                    <tr valign="middle" height="41">
                                                        <td class="gametextRed">
                                                            Jacks or Better<br>
                                                            25 Lines!<br>
                                                            Min. Bet: $2.50, Max. Bet: $31.25
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </td>
                                        <td class="casinogames">
                                            <table class="casinogame" cellspacing="0" cellpadding="0">
                                                <tbody>
                                                    <tr valign="bottom">
                                                        <td valign="top" class="gameiconRed">
                                                            <a href="#" onclick="OnGameClicked();">
                                                                <img border="0" class="lobbyimg" alt="Video Poker - Deuces Wild - 25 Line" src="/images/DeucesWild25-VideoPoker.jpg"></a>
                                                        </td>
                                                    </tr>
                                                    <tr valign="middle" height="41">
                                                        <td class="gametextRed">
                                                            Deuces Wild<br>
                                                            25 Lines!<br>
                                                            Min. Bet: $2.50, Max. Bet: $31.25
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </td>
                                        <td class="casinogames">
                                            <table class="casinogame" cellspacing="0" cellpadding="0">
                                                <tbody>
                                                    <tr valign="bottom">
                                                        <td valign="top" class="gameiconRed">
                                                            <a href="#" onclick="OnGameClicked();">
                                                                <img border="0" class="lobbyimg" alt="Video Poker - Jokers Wild - 25 Line" src="/images/Jokers-Wild25-VideoPoker.jpg"></a>
                                                        </td>
                                                    </tr>
                                                    <tr valign="middle" height="41">
                                                        <td class="gametextRed">
                                                            Jokers Wild<br>
                                                            25 Lines!<br>
                                                            Min. Bet: $2.50, Max. Bet: $31.25
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="casinogames">
                                            <table class="casinogame" cellspacing="0" cellpadding="0">
                                                <tbody>
                                                    <tr valign="bottom">
                                                        <td valign="top" class="gameiconBlue">
                                                            <a href="#" onclick="OnGameClicked();">
                                                                <img border="0" class="lobbyimg" alt="Slots - Touchdown Fever" src="/images/TouchdownFever-Slot.jpg"></a>
                                                        </td>
                                                    </tr>
                                                    <tr valign="middle" height="41">
                                                        <td class="gametextBlue">
                                                            Touchdown Fever<br>
                                                            Dollar Slots<br>
                                                            Min. Bet: $1, Max. Bet: $3
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </td>
                                        <td class="casinogames">
                                            <table class="casinogame" cellspacing="0" cellpadding="0">
                                                <tbody>
                                                    <tr valign="bottom">
                                                        <td valign="top" class="gameiconBlue">
                                                            <a href="#" onclick="OnGameClicked();">
                                                                <img border="0" class="lobbyimg" alt="Slots - Jurassic Fire" src="/images/JurassicFire-slot.jpg"></a>
                                                        </td>
                                                    </tr>
                                                    <tr valign="middle" height="41">
                                                        <td class="gametextBlue">
                                                            Jurassic Fire<br>
                                                            Quarter Slots<br>
                                                            Min. Bet: 25¢, Max. Bet: 75¢
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </td>
                                        <td class="casinogames">
                                            <table class="casinogame" cellspacing="0" cellpadding="0">
                                                <tbody>
                                                    <tr valign="bottom">
                                                        <td valign="top" class="gameiconBlue">
                                                            <a href="#" onclick="OnGameClicked();">
                                                                <img border="0" class="lobbyimg" alt="Slots - Wild 7s" src="/images/Wild7s-slot.jpg"></a>
                                                        </td>
                                                    </tr>
                                                    <tr valign="middle" height="41">
                                                        <td class="gametextBlue">
                                                            Slots - Wild 7s<br>
                                                            Min. Bet: $1, Max. Bet: $15
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="casinogames">
                                            <table class="casinogame" cellspacing="0" cellpadding="0">
                                                <tbody>
                                                    <tr valign="bottom">
                                                        <td valign="top" class="gameiconBlue">
                                                            <a href="#" onclick="OnGameClicked();">
                                                                <img border="0" class="lobbyimg" alt="Five-Reel Fruity Fortune Slots" src="/images/FruityFortune-Slot.jpg"></a>
                                                        </td>
                                                    </tr>
                                                    <tr valign="middle" height="41">
                                                        <td class="gametextBlue">
                                                            Five-Reel Fruity Fortune Slots<br>
                                                            Min. Bet: 25¢, Max. Bet: $45
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </td>
                                        <td class="casinogames">
                                            <table class="casinogame" cellspacing="0" cellpadding="0">
                                                <tbody>
                                                    <tr valign="bottom">
                                                        <td valign="top" class="gameiconBlue">
                                                            <a href="#" onclick="OnGameClicked();">
                                                                <img border="0" class="lobbyimg" alt="Five-Reel Serpent&#39;s Treasure Slots" src="/images/SerpentsTreasure-Slot.jpg"></a>
                                                        </td>
                                                    </tr>
                                                    <tr valign="middle" height="41">
                                                        <td class="gametextBlue">
                                                            Serpent's Treasure Slots<br>
                                                            Min. Bet: 25¢, Max. Bet: $45
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </td>
                                        <td class="casinogames">
                                            <table class="casinogame" cellspacing="0" cellpadding="0">
                                                <tbody>
                                                    <tr valign="bottom">
                                                        <td valign="top" class="gameiconBlue">
                                                            <a href="#" onclick="OnGameClicked();">
                                                                <img border="0" class="lobbyimg" alt="Slots - Pirate&#39;s Revenge" src="/images/PiratesRevenge-Slot.jpg"></a>
                                                        </td>
                                                    </tr>
                                                    <tr valign="middle" height="41">
                                                        <td class="gametextBlue">
                                                            Pirate's Revenge Slots<br>
                                                            Min. Bet: 5¢, Max. Bet: $2.25
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="casinogames">
                                            <table class="casinogame" cellspacing="0" cellpadding="0">
                                                <tbody>
                                                    <tr valign="bottom">
                                                        <td valign="top" class="gameiconBlue">
                                                            <a href="#" onclick="OnGameClicked();">
                                                                <img border="0" class="lobbyimg" alt="Slots - Victory Lane" src="/images/SL3x3VL.jpg"></a>
                                                        </td>
                                                    </tr>
                                                    <tr valign="middle" height="41">
                                                        <td class="gametextBlue">
                                                            3x3 Victory Lane Slots<br>
                                                            Min. Bet: 5¢, Max. Bet: $1
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </td>
                                        <td class="casinogames">
                                            <table class="casinogame" cellspacing="0" cellpadding="0">
                                                <tbody>
                                                    <tr valign="bottom">
                                                        <td valign="top" class="gameiconBlue">
                                                            <a href="#" onclick="OnGameClicked();">
                                                                <img border="0" class="lobbyimg" alt="Five-Reel Bounty Hunter Slots" src="/images/SL5RBH.jpg"></a>
                                                        </td>
                                                    </tr>
                                                    <tr valign="middle" height="41">
                                                        <td class="gametextBlue">
                                                            Five-Reel Bounty Hunter Slots<br>
                                                            Min. Bet: 25¢, Max. Bet: $9
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </td>
                                        <td class="casinogames">
                                            <table class="casinogame" cellspacing="0" cellpadding="0">
                                                <tbody>
                                                    <tr valign="bottom">
                                                        <td valign="top" class="gameiconBlue">
                                                            <a href="#" onclick="OnGameClicked();">
                                                                <img border="0" class="lobbyimg" alt="Slots - Knight&#39;s Conquest" src="/images/SL5RKC.jpg"></a>
                                                        </td>
                                                    </tr>
                                                    <tr valign="middle" height="41">
                                                        <td class="gametextBlue">
                                                            Knight's Conquest Slots<br>
                                                            Min. Bet: 25¢, Max. Bet: $9
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="casinogames">
                                            <table class="casinogame" cellspacing="0" cellpadding="0">
                                                <tbody>
                                                    <tr valign="bottom">
                                                        <td valign="top" class="gameiconBlue">
                                                            <a href="#" onclick="OnGameClicked();">
                                                                <img border="0" class="lobbyimg" alt="Slots - Retro Sci-Fi" src="/images/SL5RSF.jpg"></a>
                                                        </td>
                                                    </tr>
                                                    <tr valign="middle" height="41">
                                                        <td class="gametextBlue">
                                                            Five-Reel Retro Sci-Fi Slots<br>
                                                            Min. Bet: 5¢, Max. Bet: $9
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </td>
                                        <td class="casinogames">
                                            <table class="casinogame" cellspacing="0" cellpadding="0">
                                                <tbody>
                                                    <tr valign="bottom">
                                                        <td valign="top" class="gameiconBlue">
                                                            <a href="#" onclick="OnGameClicked();">
                                                                <img border="0" class="lobbyimg" alt="Slots - Forest Fairies" src="/images/SL5RFR.jpg"></a>
                                                        </td>
                                                    </tr>
                                                    <tr valign="middle" height="41">
                                                        <td class="gametextBlue">
                                                            Five-Reel Forest Fairies Slots<br>
                                                            Min. Bet: 5¢, Max. Bet: $45
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </td>
                                        <td class="casinogames">
                                            <table class="casinogame" cellspacing="0" cellpadding="0">
                                                <tbody>
                                                    <tr valign="bottom">
                                                        <td valign="top" class="gameiconBlue">
                                                            <a href="#" onclick="OnGameClicked();">
                                                                <img border="0" class="lobbyimg" alt="Slots - Lost Ruins" src="/images/SL5RLR.jpg"></a>
                                                        </td>
                                                    </tr>
                                                    <tr valign="middle" height="41">
                                                        <td class="gametextBlue">
                                                            Five-Reel Lost Ruins Slots<br>
                                                            Min. Bet: 5¢, Max. Bet: $45
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="casinogames">
                                            <table class="casinogame" cellspacing="0" cellpadding="0">
                                                <tbody>
                                                    <tr valign="bottom">
                                                        <td valign="top" class="gameiconBlue">
                                                            <a href="#" onclick="OnGameClicked();">
                                                                <img border="0" class="lobbyimg" alt="Slots - 5-Reel Fire" src="/images/SL5R5F.jpg"></a>
                                                        </td>
                                                    </tr>
                                                    <tr valign="middle" height="41">
                                                        <td class="gametextBlue">
                                                            5-Reel Fire Slots<br>
                                                            Min. Bet: 5¢, Max. Bet: $45
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </td>
                                        <td class="casinogames">
                                            <table class="casinogame" cellspacing="0" cellpadding="0">
                                                <tbody>
                                                    <tr valign="bottom">
                                                        <td valign="top" class="gameiconBlue">
                                                            <a href="#" onclick="OnGameClicked();">
                                                                <img border="0" class="lobbyimg" alt="Slots - Barnyard Bucks" src="/images/SL5RBB.jpg"></a>
                                                        </td>
                                                    </tr>
                                                    <tr valign="middle" height="41">
                                                        <td class="gametextBlue">
                                                            Barnyard Bucks Slots<br>
                                                            Min. Bet: 5¢, Max. Bet: $45
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </td>
                                        <td class="casinogames">
                                            <table class="casinogame" cellspacing="0" cellpadding="0">
                                                <tbody>
                                                    <tr valign="bottom">
                                                        <td valign="top" class="gameiconBlue">
                                                            <a href="#" onclick="OnGameClicked();">
                                                                <img border="0" class="lobbyimg" alt="Video Keno" src="/images/VKENO.jpg"></a>
                                                        </td>
                                                    </tr>
                                                    <tr valign="middle" height="41">
                                                        <td class="gametextBlue">
                                                            Video Keno<br>
                                                            Min. Bet: 10¢, Max. Bet: $5
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="casinogames">
                                            <table class="casinogame" cellspacing="0" cellpadding="0">
                                                <tbody>
                                                    <tr valign="bottom">
                                                        <td valign="top" class="gameiconBlue">
                                                            <a href="#" onclick="OnGameClicked();">
                                                                <img border="0" class="lobbyimg" alt="Video Bingo" src="/images/VBINGO.jpg"></a>
                                                        </td>
                                                    </tr>
                                                    <tr valign="middle" height="41">
                                                        <td class="gametextBlue">
                                                            Video Bingo - 8 Cards<br>
                                                            Min. Bet: 25¢, Max. Bet: $40
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                            <div align="center">
                            </div>
                        </center>
                            </div>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>

</asp:Content>
