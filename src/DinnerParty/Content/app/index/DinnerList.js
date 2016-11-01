import React from 'react'
import moment from 'moment'
import './DinnerList.css'

export default function DinnerList({dinners}) {
  const list = dinners.map((dinner, i)=> (
    <li key={i}>
      <a href={dinner.url}>{dinner.title}</a><br/>
      <strong>{moment(dinner.eventDate).format('MMM d')}</strong> with {dinner.rsvpCount} RSVP
    </li>
  ))
  return (
    <ul>
      {list}
    </ul>
  )
}
