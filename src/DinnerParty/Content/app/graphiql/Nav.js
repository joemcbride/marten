import React, { Component, PropTypes } from 'react'

class Nav extends Component {

  constructor(props) {
    super(props)

    const route = props.routes.length > 0
      ? props.routes[0]
      : null

    this.state = {
      selected: route
    }
  }

  static propTypes = {
    routes: PropTypes.array.isRequired
  }

  onSelectRoute(e, route) {
    e.preventDefault()
    this.setState({
      selected: route
    })
  }

  render() {
    const { selected } = this.state
    const { className } = this.props

    const routes = this.props.routes.map((r, i)=> {
      return (
        <li key={i}>
          <a href="#" onClick={(e)=> this.onSelectRoute(e, r)}>{r.name}</a>
        </li>
      )
    })
    return (
      <div className={className}>
        <ul className="nav-list">
          {routes}
        </ul>
        {selected.component}
      </div>
    )
  }
}

export default Nav
