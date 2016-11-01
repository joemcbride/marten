import React, { Component, PropTypes } from 'react'
import ReactDOM, { findDOMNode } from 'react-dom'

class BingMap extends Component {

  static defaultProps = {
    height: '450px',
    width: '580px'
  }

  state = {
    map: null
  }

  componentDidMount() {
    const { latitude, longitude } = this.props
    const element = findDOMNode(this)

    const map = new VEMap('someId')
    const options = new VEMapOptions()
    options.EnableBirdseye = false

    map.SetDashboardSize(VEDashboardSize.Small)

    let center = null

    if (latitude && longitude) {
      center = new VELatLong(latitude, longitude)
    }

    map.LoadMap(center, null, null, null, null, null, null, options)

    this.setState({ map })
  }

  render() {
    const { className, height, width } = this.props;

    const style = {
      position: 'relative',
      height: height,
      width: width,
      border: 'solid 1px #bbd1ec'
    }

    return (
      <div id="someId" className={className} style={style}>
        the map
      </div>
    )
  }
}

export default BingMap
