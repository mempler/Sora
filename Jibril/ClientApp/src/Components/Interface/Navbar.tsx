import * as React from 'react';
import i18n from '../I18n';
import { Link } from 'react-router-dom';

// @ts-ignore
import logo from '../../Assets/Images/Interface/logo.png'

// TEMP Variables until i've finished the javascript shit.
const USERNAME = "TEST";
const USERID = 0;
const PATH = "/";
const MSG = 0;
const GLOBRANK = 0;
const PP = 0;
const ACC = 0;
const LVL = 0;
const IsLoggedIn = false;

const logout = () => {

}

class Navbar extends React.Component {
  Profile() {
    if (IsLoggedIn)
      return (
        <div id="profile">
          <div id="username">
            <span style={{ fontSize: '18px' }}>{USERNAME}</span>
          </div>
          <div id="sessionControls">
            <a href="" onClick={logout}>{i18n.t("logout")}</a> &bull; <Link to="/settings">{i18n.t("settings")}></Link>
          </div>
          <div id="stats">
            <span style={{ fontSize: '16px', fontWeight: 'bold' }}>#{GLOBRANK}</span> <br />
            {PP}PP<br />
            {ACC}% Accuracy<br />
            Level {LVL}
          </div>
        </div>
      )
    else
      return (
        <div id="profile">
          <div id="welcome">
            <span style={{ fontSize: '15px' }}>{i18n.t("welcome_guest")}</span> <br />
            <span>Login &bull; <Link to="/signup" >{i18n.t("signup")}</Link></span>
          </div>
          { /* TODO: add design here */}
          <div id="login" />
        </div>
      )
  }
  Avatar() {
    if (IsLoggedIn)
      return (
        <div id="navAvatar">
          <Link to={"/u/" + USERID}><img src={"/" + USERID} alt="Your user profile." /></Link>
        </div>
      )
    else
      return (
        <div id="navAvatar">
          <img src="/0" width="56" height="56" alt="Default osu profile." />
        </div>
      )
  }
  Messages() {
    if (IsLoggedIn)
      return <Link to="/messages"><span className="right">{/* TODO: Make this do stuff */}<i className="fa fa-envelope" aria-hidden="true"></i> {MSG} {i18n.t("messages")}</span></Link>
    return <div />
  }
  render() {
    return (
      <div className="navbar">
        <div id="navbar">
          <div id="navContents">
            <Link to="/home" id="logo">
              <span style={{ display: 'inline-block', fontSize: "50px", position: 'absolute' }}> {/* <i className="fas fa-bullseye"></i> */} <img src={logo} height="60" alt="" /></span>
            </Link>
            <form method="get" action="https://gigamons.de/" id="searchbox">
              <input type="text" name="q" size={31} value="" autoComplete="off" placeholder={i18n.t('search') as string} />
            </form>
            <div id="leftNav">
              <div id="leftnavlinks">
                <span className="navlink"><Link to="/help">{i18n.t("help")} <i className="fa fa-caret-right navCaretMargin" aria-hidden="true"></i></Link></span>
                <span className="navlink"><Link to="/leaderboard">{i18n.t("leaderboard")} <i className="fa fa-caret-right navCaretMargin" aria-hidden="true"></i></Link></span>
                <span className="navlink"><Link to="/beatmaps">{i18n.t("beatmaps")} <i className="fa fa-caret-right navCaretMargin" aria-hidden="true"></i></Link></span>
              </div>
            </div>
            <div id="rightNav">
              <div id="rightnavlinks">
                <this.Profile />
              </div>
              <this.Avatar />
            </div>
          </div>
          <div id="advanced">
            <div id="advancedNested">
              <Link to="switcher.exe"><div className="download">{i18n.t("dl_switcher")}</div></Link>
              <div className="nav">
                <Link to="/"><span>Gigamons!</span></Link> &#187; <Link to={"/"}><span>{PATH}</span></Link>
              </div>
              <div className="rightSide">
                <this.Messages />
              </div>
            </div>
          </div>
        </div>
      </div>
    )
  }
}

export default Navbar