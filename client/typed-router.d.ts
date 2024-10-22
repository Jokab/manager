/* eslint-disable */
/* prettier-ignore */
// @ts-nocheck
// Generated by unplugin-vue-router. ‼️ DO NOT MODIFY THIS FILE ‼️
// It's recommended to commit this file.
// Make sure to add this file to your tsconfig.json file as an "includes" or "files" entry.

declare module 'vue-router/auto-routes' {
  import type {
    RouteRecordInfo,
    ParamValue,
    ParamValueOneOrMore,
    ParamValueZeroOrMore,
    ParamValueZeroOrOne,
  } from 'vue-router'

  /**
   * Route name map generated by unplugin-vue-router
   */
  export interface RouteNamedMap {
    '/': RouteRecordInfo<'/', '/', Record<never, never>, Record<never, never>>,
    '/create-manager': RouteRecordInfo<'/create-manager', '/create-manager', Record<never, never>, Record<never, never>>,
    '/create-team': RouteRecordInfo<'/create-team', '/create-team', Record<never, never>, Record<never, never>>,
    '/drafts/[id]': RouteRecordInfo<'/drafts/[id]', '/drafts/:id', { id: ParamValue<true> }, { id: ParamValue<false> }>,
    '/leagues/': RouteRecordInfo<'/leagues/', '/leagues', Record<never, never>, Record<never, never>>,
    '/leagues/[id]': RouteRecordInfo<'/leagues/[id]', '/leagues/:id', { id: ParamValue<true> }, { id: ParamValue<false> }>,
    '/login': RouteRecordInfo<'/login', '/login', Record<never, never>, Record<never, never>>,
    '/managers/[id]': RouteRecordInfo<'/managers/[id]', '/managers/:id', { id: ParamValue<true> }, { id: ParamValue<false> }>,
    '/teams/[id]': RouteRecordInfo<'/teams/[id]', '/teams/:id', { id: ParamValue<true> }, { id: ParamValue<false> }>,
  }
}
