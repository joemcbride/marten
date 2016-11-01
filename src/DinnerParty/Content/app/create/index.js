import React, { PropTypes } from 'react'
import { connect } from 'react-redux'
import { graphql, compose } from 'react-apollo'
import gql from 'graphql-tag'

import App from '../app'
import BingMap from '../BingMap'
import DinnerForm from './DinnerForm'

import './index.css'

function Index({ submit }) {
  const latitude = 38.59072652516204
  const longitude = -117.02004241943357
  return (
    <div>
      <BingMap className="pull-left" latitude={latitude} longitude={longitude} width="522px"/>
      <div className="pull-left dinner-list">
        <DinnerForm onSubmit={submit}/>
      </div>
    </div>
  )
}

Index.propTypes = {
  submit: PropTypes.func.isRequired
}

const createDinner = gql`
  mutation newDinner(
    $title: String!,
    $description: String!,
    $date: Date!,
    $address: String!,
    $contactPhone: String!) {
    createDinner(dinner:
      {
        title: $title
        description: $description
        eventDate: $date
        address: $address
        contactPhone: $contactPhone
      }) {
      id
      title
      eventDate
    }
  }
  `

const submitAndMutate = (mutate) => ({
  submit: ({ values }) =>
    mutate({
      variables: { values },
    }).catch(e => {
      console.error(e)
    })
});

const withMutations = graphql(createDinner, {
  options: { },
  props: ({ ownProps, mutate }) => submitAndMutate(mutate),
});

App(
  withMutations(Index)
)
