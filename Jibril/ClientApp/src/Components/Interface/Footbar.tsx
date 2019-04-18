import * as React from 'react';

// @ts-ignore
import DiscordImage from '../../Assets/Images/Interface/discord.svg'

class footbar extends React.Component {
  OpenDiscord() {
    return window.open('https://discord.gg/pZBpFjA')
  }
  render() {
    return (
      <div className="Footer">
        <div id="footer">
          <img id="discord" src={DiscordImage} onClick={this.OpenDiscord} alt=""/>
          &copy; 2017 - { (new Date()).getFullYear() } MIT &bull; <a href="https://github.com/Gigamons">Github</a><br /> <a href="https://mempler.de/">Mempler.de</a> &bull; <a href="https://jse.io/">JSE.io</a> &bull; <a href="https://p.datadoghq.com/sb/03868a281-7726fee3626d22dbcbb49dfb77cf85bb">Status</a>
        </div>
      </div>
    )
  }
}

export default footbar