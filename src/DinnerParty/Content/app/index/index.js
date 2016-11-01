import React, { PropTypes } from 'react'
import { graphql, compose } from 'react-apollo'
import gql from 'graphql-tag'

import App from '../app'
import DinnerList from './DinnerList'
import BingMap from '../BingMap'
import './index.css'

function Index({data}) {
  const popularDinners = data.popularDinners || []
  const latitude = 38.59072652516204
  const longitude = -117.02004241943357
  return (
    <div>
      <BingMap className="pull-left" latitude={latitude} longitude={longitude}/>
      <div className="pull-left dinner-list">
        <h2>Popular Dinners</h2>
        <DinnerList dinners={popularDinners}/>
  	    <a rel="feedurl" href="/Dinners/WebSlicePopular" style={{display:"none"}}></a>
      </div>
    </div>
  )
}

Index.propTypes = {
  data: PropTypes.shape({
    loading: PropTypes.bool.isRequired,
    popularDinners: PropTypes.array,
  }).isRequired
}

const query = gql`
  query dinnerQuery($limit: Int) {
    popularDinners(limit: $limit) {
      id
      url
      title
      description
      eventDate
      latitude
      longitude
      rsvpCount
    }
  }`

App(
  compose(
    graphql(query, {
      options: { variables: { limit: 8 } }
    })
  )(Index)
)
