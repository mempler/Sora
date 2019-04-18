import React from 'react';
import i18n from '../I18n';

import megumin from '../../Assets/Images/Interface/Megumin.png';

class SignupPage extends React.Component {
  render() {
    return (
      <div className="SignupPage">
        <div className="signupBlock">
          <h1 style={{ margin: 0, padding: 0, paddingTop: "8px", color: "#4B4C52" }}>{i18n.t("signup_prepare")}</h1>
          <h2 style={{ fontSize: "16px", color: "rgb(65, 65, 65)", fontWeight: 100 }}>{i18n.t("signup_description")}</h2>
        </div>
        <div className="signupBlock">
          <table>
            <tbody>
              <tr>
                <td>
                  {i18n.t("signup_fun")}
                  <br />
                  <img src={megumin} height="400" alt="" />
                </td>
                <td style={{ width: "600px" }}>
                  {/* FIXME: Use another className instead of signupForm */}
                  <div className="signupForm hiddenLoading" style={{ color: "green" }}>
                    <div className="loading" style={{ position: "relative", marginLeft: "45%", marginBottom: "1%" }} /> {i18n.t("signup_verification_email_send")} <br /> {i18n.t("signup_dont_close")}
                  </div>
                  <form className="signupForm">
                    <div id="signupFormContents">
                      <input type="text" name="username" size={31} value="" placeholder={i18n.t('signup_username') as string} />
                      <br />
                      <input type="email" name="email" size={31} value="" placeholder={i18n.t('signup_email') as string} />
                      <br />
                      <input type="password" name="password" size={31} value="" placeholder={i18n.t('signup_password') as string} />
                      <br />
                      <input type="password" name="confirm" size={31} value="" placeholder={i18n.t('signup_confirm') as string} />
                      <br />
                      <input type="button" className="g-recaptcha" data-sitekey="config.GoogleKey" data-callback="signup" name="submit" size={31} value={"" +i18n.t('signup_create_account')} />
                    </div>
                  </form>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
        <div className="signupBlock signupNotice">
          <i className="fa fa-exclamation-triangle" aria-hidden="true"></i>
          <span>{i18n.t("signup_step_o")}</span>
          <i className="fa fa-exclamation-triangle" aria-hidden="true"></i>
          <table style={{ tableLayout: "fixed" }}>
            <tbody>
              <tr>
                <td style={{ width: "33%", verticalAlign: "top" }}>
                  <h3>
                    <i className="fa fa-envelope" aria-hidden="true"></i> &nbsp; &nbsp;{i18n.t("signup_step_number")} 1</h3>
                  <div style={{ color: "black", fontSize: "15px", marginTop: "10px", textShadow: "none" }}>{i18n.t("signup_step_one")}</div>
                </td>
                <td style={{ width: "33%" }}>
                  <h3>
                    <i className="fa fa-download" aria-hidden="true"></i> &nbsp; &nbsp;{i18n.t("signup_step_number")} 2
                    <br />
                  </h3>
                  <div style={{ color: "black", fontSize: "15px", marginTop: "10px", textShadow: "none" }}>{i18n.t("signup_step_two")}</div>
                </td>
                <td style={{ width: "33%" }}>
                  <h3>
                    <i className="fa fa-user" aria-hidden="true"></i> &nbsp; &nbsp;{i18n.t("signup_step_number")} 3
                    <br />
                  </h3>
                  <div style={{ color: "black", fontSize: "15px", marginTop: "10px", textShadow: "none" }}>{i18n.t("signup_step_three")}</div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    )
  }
}

export default SignupPage