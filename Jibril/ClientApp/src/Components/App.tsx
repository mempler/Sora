import * as React from 'react'
import { BrowserRouter, Route } from 'react-router-dom'

// Interface
import Navbar from './Interface/Navbar'
import Footbar from './Interface/Footbar'

// Styling
import '../Assets/Styles/Default/index.scss'

// Content
import News from './Content/News'
import Signup from './Content/Signup'

class App extends React.Component {
  render() {
    return (
      <BrowserRouter >
        <div className="App">
          <Navbar />
          <div id="container">
            <div id="content" style={{ textAlign: "center" }}>
              <div>
                <Route path="/" component={News} />
                <Route path="/signup" component={Signup} />
              </div>
            </div>
          </div>
          <Footbar />
        </div>
      </BrowserRouter>
    );
  }
}

export default App;