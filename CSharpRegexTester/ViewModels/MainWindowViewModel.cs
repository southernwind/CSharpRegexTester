using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text.RegularExpressions;

using Livet;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace CSharpRegexTester.ViewModels {
	internal class MainWindowViewModel : ViewModel {
		/// <summary>
		/// 対象文字列
		/// </summary>
		public ReactivePropertySlim<string> Text {
			get;
		} = new ReactivePropertySlim<string>("");

		/// <summary>
		/// 正規表現
		/// </summary>
		public ReactiveProperty<string> Format {
			get;
		} = new ReactiveProperty<string>("");

		/// <summary>
		/// 置換文字列
		/// </summary>
		public ReactivePropertySlim<string> Replacement {
			get;
		} = new ReactivePropertySlim<string>("");

		/// <summary>
		/// マッチ結果
		/// </summary>
		public ReactivePropertySlim<IEnumerable<Match>> MatchResult {
			get;
		} = new ReactivePropertySlim<IEnumerable<Match>>();

		/// <summary>
		/// 置換後文字列
		/// </summary>
		public ReactivePropertySlim<string> ReplacedText {
			get;
		} = new ReactivePropertySlim<string>();

		/// <summary>
		/// オプション候補
		/// </summary>
		public ReactiveCollection<RegexOptionWithEnabledFlag> CandidateRegexOptions {
			get;
		} = new ReactiveCollection<RegexOptionWithEnabledFlag>();

		public MainWindowViewModel() {
			this.Format.SetValidateNotifyError(x => {
				try {
					new Regex(x);
				} catch (Exception) {
					return "正規表現フォーマットエラー";
				}
				return null;
			});
			foreach (var ro in Enum.GetValues(typeof(RegexOptions)).Cast<RegexOptions>()) {
				this.CandidateRegexOptions.Add(new RegexOptionWithEnabledFlag(ro, false));
			}

			Observable.Merge(
				this.Text,
				this.Format,
				this.Replacement).ToUnit()
				.Merge(this.Format.ObserveHasErrors.ToUnit())
				.Merge(this.CandidateRegexOptions.ObserveElementObservableProperty(x => x.Enabled).ToUnit())
				.Where(_ => !this.Format.HasErrors)
				.Subscribe(_ => {
					var options = RegexOptions.None;
					foreach (var op in this.CandidateRegexOptions.Where(x => x.Enabled.Value)) {
						options |= op.RegexOption;
					}
					var regex = new Regex(this.Format.Value, options);
					this.ReplacedText.Value = regex.Replace(this.Text.Value, this.Replacement.Value);
					this.MatchResult.Value = regex.Matches(this.Text.Value).AsEnumerable();
				});
		}
	}

	internal class RegexOptionWithEnabledFlag : NotificationObject {
		public RegexOptions RegexOption {
			get;
		}

		public string DisplayName {
			get {
				return Enum.GetName(typeof(RegexOptions), this.RegexOption);
			}
		}

		public ReactivePropertySlim<bool> Enabled {
			get;
		} = new ReactivePropertySlim<bool>();

		public RegexOptionWithEnabledFlag(RegexOptions regexOption, bool enabled) {
			this.RegexOption = regexOption;
			this.Enabled.Value = enabled;
		}
	}
}
